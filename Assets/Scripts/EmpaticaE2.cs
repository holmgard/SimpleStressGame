using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public enum EmpaticaState
	{
		fresh,
		scanning,	
		scanned,
		opening,
		ready,
		starting,
		started,
		receiving,
		stopping,
		stopped,
		closing,
		closed,
		shutdown,
		error,
		nodevice
	}

public class EmpaticaE2 : MonoBehaviour {

	public string empaticaServerIP = "127.0.0.1";
	public int serverPort = 27000;
	private TcpClient client;
	private NetworkStream stream;
	private Encoding encoding;
	
	public EmpaticaState empState = EmpaticaState.fresh;
	
	private Thread receiveThread;
	private Thread fillBufferThread;
	private byte[] receivingBuffer;
	public int bufferSize = 1024;
	
	private Thread saveThread;
	
	public volatile ArrayList dataStorage;
	
	private byte[] data;
	
	void Start()
	{
		Application.runInBackground = true;
		encoding = new System.Text.ASCIIEncoding();
		dataStorage = new ArrayList();
	}
	
	public void ConnectEmpatica()
	{
		Debug.Log("Connecting TCP client");
		client = new TcpClient(empaticaServerIP,serverPort);
		Debug.Log("Getting stream");
		stream = client.GetStream();
		stream.ReadTimeout = Timeout.Infinite;
		print ("Reading timeout: " + stream.ReadTimeout.ToString());
		print ("Starting thread");
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.Start();
	}
	
	public void DisconnectEmpatica()
	{
		empState = EmpaticaState.stopping;
	}
	
	public void ReceiveData()
	{
		while(true)
		{
			switch(empState)
			{
			case EmpaticaState.fresh:
				empState = EmpaticaState.scanning;
				empState = ScanForEmpatica();
				break;
			case EmpaticaState.scanning:
				break;
			case EmpaticaState.scanned:
				empState = EmpaticaState.opening;
				empState = OpenEmpatica();
				break;
			case EmpaticaState.opening:
				break;
			case EmpaticaState.ready:
				empState = EmpaticaState.starting;
				empState = StartEmpatica();
				break;
			case EmpaticaState.starting:
				break;
			case EmpaticaState.started:
				empState = StartReceivingData();
				break;
			case EmpaticaState.receiving:
				dataStorage.AddRange(ReadSamples());
				if(dataStorage.Count % 100 == 0)
				{
					print ("Collected samples: " + dataStorage.Count.ToString());
				}
				break;
			case EmpaticaState.stopping:
				empState = StopEmpatica();
				break;
			case EmpaticaState.stopped:
				empState = CloseEmpatica();
				break;
			case EmpaticaState.closing:
				break;
			case EmpaticaState.closed:
				empState = ShutDownConnection();
				break;
			case EmpaticaState.nodevice:
				break;
			case EmpaticaState.error:
				break;
			default:
				break;
			}
		}
	}
	
	void TempReceive()
	{
		print ("TempReceive In");
		for(int i = 0; i < 1000; i++)
		{
			print (i);
		}
		empState = EmpaticaState.stopping;
		print ("TempReceive Out");
	}
	
	private EmpaticaState ScanForEmpatica()
	{
		print ("ScanForEmpatica()");
		if(stream.DataAvailable){
			print(ReadText());
		}
		stream.Write(encoding.GetBytes("scan"),0,4);
		string result = ReadText();
		if(result.Contains("OK"))
		{
			return EmpaticaState.scanned;
		} else if (result.Contains("ER"))
		{
			return EmpaticaState.nodevice;
		} else
		{
			return EmpaticaState.error;
		}
	}
	
	private EmpaticaState OpenEmpatica()
	{
		print ("OpenEmpatica()");
		stream.Write(encoding.GetBytes("open"),0,4);
		string result = ReadText();
		print("From open " + result);
		if(result.Contains("OK"))
		{
			return EmpaticaState.ready;
		} else if (result.Contains("ER"))
		{
			return EmpaticaState.error;
		} else
		{
			return EmpaticaState.error;
		}
	}
	
	private EmpaticaState StartEmpatica()
	{
		print ("StartEmpatica()");
		stream.Write(encoding.GetBytes("start"),0,5);
		string result = ReadText();
		if(result.Contains("OK"))
		{
			return EmpaticaState.started;
		} else if (result.Contains("ER"))
		{
			return EmpaticaState.error;
		} else
		{
			return EmpaticaState.error;
		}
	}
	
	private EmpaticaState StopEmpatica()
	{
		print ("StopEmpatica()");
		EmpaticaState result = EmpaticaState.error;
		stream.Write(encoding.GetBytes("stop"),0,4);
		int stoppingBuffer = 1000;
		int iterations = 0;
		bool done = false;
		string response = "";
		while(iterations < stoppingBuffer && !done)
		{
			response = ReadText();
			iterations++;
			if(response.Contains("OK"))
			{
				done = true;
				result = EmpaticaState.stopped;
			} else if(response.Contains("ER"))
			{
				done = true;
				result = EmpaticaState.error;
			} else {
				result = EmpaticaState.error;
			}
		}
		return result;
	}
	
	private EmpaticaState CloseEmpatica()
	{
		print ("CloseEmpatica");
		stream.Write(encoding.GetBytes("close"),0,5);
		string result = ReadText();
		if(result.Contains("OK"))
		{
			return EmpaticaState.closed;
		} else if (result.Contains("ER"))
		{
			return EmpaticaState.error;
		} else
		{
			return EmpaticaState.error;
		}
	}
	
	private EmpaticaState ShutDownConnection()
	{
		print ("ShutDownConnection");
		stream.Close();
		client.Close();
		receiveThread.Abort();
		return EmpaticaState.shutdown;
	}
	
	private string ReadText()
	{
		string result = "";
		data = new byte[4];
		stream.Read(data,0,data.Length);
		result = encoding.GetString(data);
		print ("From ReadText: " + result);
		return result;
	}
	
	private EmpaticaState StartReceivingData()
	{
		receivingBuffer = new byte[bufferSize];
		FillBuffer(bufferSize);
		return EmpaticaState.receiving;
	}
	
	private ArrayList ReadSamples()
	{
		ArrayList result = new ArrayList();
		
		if(receivingBuffer != null && stream.DataAvailable)
		{	
			int channelID = (int)receivingBuffer[0];
			int dataLength = (int)receivingBuffer[1];
			
			int samplesLengthInBytes = dataLength*4 + 2;
			
			print (GetSampleType(channelID).ToString() + " " + dataLength.ToString());
			
			for(int i = 0; i < dataLength; i++)
			{
				//Reversing bytes
				byte[] valueBytes = new byte[4];
				byte[] reversedBytes = new byte[4];
				Array.Copy(receivingBuffer, (i*4+2),valueBytes, 0,4);
				reversedBytes[0] = valueBytes[3];
				reversedBytes[1] = valueBytes[2];
				reversedBytes[2] = valueBytes[1];
				reversedBytes[3] = valueBytes[0];
				//End reversing
				
				float sampleValue = (float)BitConverter.ToSingle(reversedBytes, 0);				
				
				print(GetSampleType(channelID).ToString() + " " + sampleValue.ToString());
				
				EmpaticaSample sample = new EmpaticaSample(DateTime.Now, GetSampleType(channelID), sampleValue);
				result.Add(sample);
			}
			
			FillBuffer(samplesLengthInBytes);
		}
		return result;
	}
	
	private void FillBuffer(int oldBytes)
	{
		int bytesToKeep = bufferSize - oldBytes;
		byte[] newRead = new byte[oldBytes];
		int bytesRead = stream.Read(newRead,0, oldBytes);
		while(bytesRead < oldBytes)
		{
			int temp = stream.Read(newRead,bytesRead,oldBytes-bytesRead);
			bytesRead += temp;
		}
		Array.Copy(receivingBuffer, oldBytes, receivingBuffer, 0, bytesToKeep);
		Array.Copy(newRead, 0, receivingBuffer, bytesToKeep, newRead.Length);
	}
	
	private SampleType GetSampleType(int channelId)
	{
		SampleType result;
		switch(channelId)
		{
			case 1:
				result = SampleType.BVP;
				break;
			case 2:
				result = SampleType.GSRT;
				break;
			case 3:
				result = SampleType.GSRP;
				break;
			case 4:
				result = SampleType.ACCX;
				break;
			case 5:
				result = SampleType.ACCY;
				break;
			case 6:
				result = SampleType.ACCZ;
				break;
			case 7:
				result = SampleType.TEMP;
				break;
			case 8:
				result = SampleType.BATT;
				break;
			default:
				result = SampleType.EMPTY;
				break;
		}
		return result;
	}
	
	public void SaveData()
	{
		saveThread = new Thread(new ThreadStart(ThreadSaveData));
		saveThread.IsBackground = true;
		saveThread.Start();
	}
	
	public void ThreadSaveData() //TODO: Maybe this could be rewritten to use LINQ and be really cool?
	{
		print ("Starting save...");
		
		ArrayList bvp = new ArrayList();
		ArrayList gsrt = new ArrayList();
		ArrayList gsrp = new ArrayList();
		ArrayList accx = new ArrayList();
		ArrayList accy = new ArrayList();
		ArrayList accz = new ArrayList();
		ArrayList temp = new ArrayList();
		ArrayList batt = new ArrayList();
		ArrayList lists = new ArrayList();
		lists.Add(bvp);
		lists.Add(gsrt);
		lists.Add(gsrp);
		lists.Add(accx);
		lists.Add(accy);
		lists.Add(accz);
		lists.Add(temp);
		lists.Add(batt);
		
		foreach(EmpaticaSample sample in dataStorage)
		{
			switch(sample.type)
			{
			case SampleType.BVP:
				bvp.Add(sample);
				break;
			case SampleType.GSRT:
				gsrt.Add(sample);
				break;
			case SampleType.GSRP:
				gsrp.Add(sample);
				break;
			case SampleType.ACCX:
				accx.Add(sample);
				break;
			case SampleType.ACCY:
				accy.Add(sample);
				break;
			case SampleType.ACCZ:
				accz.Add(sample);
				break;
			case SampleType.TEMP:
				temp.Add(sample);
				break;
			case SampleType.BATT:
				batt.Add(sample);
				break;
			default:
				break;
			}
		}
		
		foreach(ArrayList list in lists)
		{
			print ("Saving...");
			if(list.Count > 0)
			{
				string fileName = "";
				fileName += DateTime.Now.Day.ToString();
				fileName += DateTime.Now.Month.ToString();
				fileName += DateTime.Now.Year.ToString();
				fileName += "_";
				fileName += DateTime.Now.Hour.ToString();
				fileName += DateTime.Now.Minute.ToString();
				fileName += DateTime.Now.Second.ToString();
				EmpaticaSample nameSample = (EmpaticaSample)list[0] as EmpaticaSample;
				fileName += "_" + nameSample.type.ToString();
				fileName += ".dat";
				
				string variableData = "";
				foreach(EmpaticaSample sample in list)
				{
					variableData += sample.ToString();
					variableData += "\n";
				}
				System.IO.File.WriteAllText(fileName,variableData);
			}
		}
		print ("Saving done");
		saveThread.Abort();
	}
	
	public void SetPort(int port)
	{
		serverPort = port;
	}
		
	public void OnApplicationQuit()
	{
		if(receiveThread != null)
			receiveThread.Abort();
		if(saveThread != null)
			saveThread.Abort();
		if(stream != null)
			stream.Close();
		if(client != null)
			client.Close();
	}
}

public enum SampleType
{
	BVP,
	GSRT,
	GSRP,
	ACCX,
	ACCY,
	ACCZ,
	TEMP,
	BATT,
	EMPTY
}

public class EmpaticaSample
{
	public DateTime time;
	public SampleType type;
	public float sampleValue;
	
	public EmpaticaSample(DateTime time, SampleType type, float sampleValue)
	{
		this.time = time;
		this.type = type;
		this.sampleValue = sampleValue;
	}
				
	public override string ToString()
	{
		return string.Format("{0:yyy-mm-dd_hh:mm:ss.fff}\t{1}\t{2}",time,type,sampleValue);
	}
}