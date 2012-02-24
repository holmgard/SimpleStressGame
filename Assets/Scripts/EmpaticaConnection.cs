using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class EmpaticaConnection : MonoBehaviour {
	
	public enum EmpaticaState{
		fresh,
		opening,
		starting,
		receiving,
		stopping,
		stopped,
		closing,
		closed,
		error
	}
		
	public EmpaticaState empState = EmpaticaState.fresh;
	
	public string empaticaServerIP = "127.0.0.1";
	public int serverPort = 27000;
	public const int signalsAvailable = 9;
	public int signalsEnabled = 2;
	
	public bool showGUI = true;
	public bool reportDuringReading = true;
	
	public float baseLine = 0;
	
	private Thread receiveThread;
	private TcpClient client;
	private NetworkStream stream;
	
	public bool empRunning = false;
	public bool sentStopSignal = false;
	
	public bool markWaiting = false;
	public string waitingMark = "";
	
	public ArrayList dataStorage; //For instances of 'EmpaticaSample'

	void Start () 
	{
		Application.runInBackground = true;
		encoding = new System.Text.ASCIIEncoding();
		dataStorage = new ArrayList();
	}
		
	public void ConnectEmpatica()
	{
		Debug.Log("Attempting connection to Empatica device");
		InitConnection();
	}
	
	public void StopEmpatica()
	{
		empState = EmpaticaState.stopping;
	}
	
	private void InitConnection()
	{
		client = new TcpClient(empaticaServerIP,serverPort);
		stream = client.GetStream();
		
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
		
		Debug.Log("Connection initiated");
	}
	
	public void OnApplicationQuit()
	{
		if(receiveThread != null && receiveThread.IsAlive)
		{
			Debug.Log("Application quitting, aborting network thread");
			receiveThread.Abort();
		}
		if(saveThread != null && saveThread.IsAlive)
		{
			Debug.Log("Application quitting, aborting save thread");
			saveThread.Abort();
		}
	}
	
	public float CalculateBaseLine()
	{
		float sum = 0;
		EmpaticaSample sample;
		for(var i = 200; i < 840; i++)
		{
			sample = (EmpaticaSample)dataStorage[i];
			sum += sample.SkinConductance;
		}
		Debug.Log("Calculated baseline");
		return sum/640;
	}
	
	public float CalculateCurrentStressLevel(ArrayList dataStorage)
	{
		//TODO put in the function from the ANN
		return 0.0F;
	}
	
	public float GetBaseline()
	{
		return baseLine;
	}
	
	public void MarkEvent(string mark)
	{
		waitingMark = mark;
		markWaiting = true;
	}
	
	#region Control flow
	private void ReceiveData()
	{
		empRunning = true;
		while(empRunning){			
			try{
				switch(empState){
				case EmpaticaState.fresh:
					Debug.Log("Receive thread running, state is fresh, trying to open Empatica connection");
					Debug.Log("Client connected: " + client.Connected);
					stream.Write(encoding.GetBytes("open"),0,4); //Use this to "open" - then "start"
					empState = EmpaticaState.opening;
					break;
					
				case EmpaticaState.opening:
					Debug.Log("Opening Empatica connection");
					if(ReadResponse() == "OK\n")
					{
						stream.Write(encoding.GetBytes("start"),0,5);
						empState = EmpaticaState.starting;
					}
					break;
					
				case EmpaticaState.starting:
					Debug.Log("Empatica connection opened, will start registering stream");
					if(ReadResponse() == "OK\n")
					{
						empState = EmpaticaState.receiving;
					}
					break;
					
				case EmpaticaState.receiving:
					if(stream.DataAvailable)
					{
						ArrayList chunk = ReadData();
						dataStorage.AddRange(chunk);
						if(reportDuringReading && dataStorage.Count%64 == 0)
								Debug.Log("Read " + dataStorage.Count.ToString() + " samples.");
						if(baseLine == 0 && dataStorage.Count > 840)
							baseLine = CalculateBaseLine();
					}
					break;
					
				case EmpaticaState.stopping:
					if(!sentStopSignal)
					{
						stream.Write(encoding.GetBytes("stop"),0,4);
						sentStopSignal = true;
					}
					
					latestData = StopReading();	
					
					if(latestData[0] as string == "OK\n")
					{
						empState = EmpaticaState.stopped;
					} else if (latestData[0] as string == "ER\n")
					{
						empState = EmpaticaState.error;
					} else
					{
						dataStorage.AddRange(latestData);
						if(reportDuringReading && dataStorage.Count%10 == 0)
								Debug.Log("Read " + dataStorage.Count.ToString() + " samples.");
					}
					break;
					
				case EmpaticaState.stopped:
					Debug.Log("Empatica stream stopped");
					stream.Write(encoding.GetBytes("close"),0,5);
					empState = EmpaticaState.closing;
					break;
					
				case EmpaticaState.closing:
					Debug.Log("Closing the Empatica connection");
					if(ReadResponse() == "OK\n")
					{
						empState = EmpaticaState.closed;
					}
					break;
					
				case EmpaticaState.closed:
					Debug.Log("Empatica connection gracefully closed - aborting the receiving thread");
					stream.Close();
					client.Close();
					receiveThread.Abort();
					break;
					
				case EmpaticaState.error:
					break;
					
				default:
					break;
				}
			} catch(SocketException e)
			{
				print(e.Message);
			}
		}
	}
	#endregion
	
	#region Modes of reading
	public byte[] rowData;
	public byte[] data;
	public int bytes;
	public string responseData;
	ArrayList dataChunk;
	public ArrayList latestData;
	public ArrayList dataRows;
	System.Text.ASCIIEncoding encoding;
	
	private string ReadResponse()
	{
		responseData = "";
		data = new byte[3];
		
		bytes = stream.Read(data,0,data.Length);
		responseData = encoding.GetString(data);
		
		if(responseData == "OK\n" || responseData == "ER\n")
		{
			Debug.Log("System response received: " + responseData);
		} else
		{
			Debug.Log("Unexpected data received: " + "<>" + responseData + "<>");
		}
		
		return responseData;
	}

	private ArrayList ReadData()
	{
		rowData = new byte[4]; //This holds the bytes that indicate the number of rows in the sample
		bytes = stream.Read(rowData,0,rowData.Length);
		
		int numRows = System.BitConverter.ToInt32(rowData,0)/2; //The actual number of rows this number is twice as large as it should be for some arcane reason
		//Debug.Log("Found this numRows: " + numRows.ToString());
		
		dataChunk = new ArrayList(numRows);
		
		data = new byte[numRows * sizeof(float) * signalsEnabled];
		//Debug.Log("Data length: " + data.Length.ToString());
		stream.Read(data,0,data.Length);
		
		for(int i = 0; i < numRows; i++)
		{
			float[] sv = new float[signalsAvailable];
			
			for(int j = 0; j < signalsEnabled; j++)
			{
				sv[j] = System.BitConverter.ToSingle(data,i * signalsEnabled * sizeof(float) + j * sizeof(float));
				
				for(int k = signalsAvailable-signalsEnabled; k < signalsAvailable; k++)
				{
					sv[k] = 0;
				}
			}
			EmpaticaSample sample;
			if(markWaiting)
			{
				sample = new EmpaticaSample(DateTime.Now,sv[0],sv[1],sv[2],sv[3],sv[4],sv[5],sv[6],sv[7],sv[8],waitingMark);
				markWaiting = false;
			} else {
				sample = new EmpaticaSample(DateTime.Now,sv[0],sv[1],sv[2],sv[3],sv[4],sv[5],sv[6],sv[7],sv[8],"");
			}
			dataChunk.Add(sample);
			//Debug.Log("i " + i.ToString() + " " + sample.ToString());
		}
		return dataChunk;
	}
	
	/* This method is kind of a kludge, and needs to be fixed,
	 * but the idea is to get all the data out of the device
	 * before stopping
	 */
	private ArrayList StopReading()
	{
		dataChunk = new ArrayList();
		
		rowData = new byte[4]; //This holds the bytes that indicate the number of rows in the sample
		byte[] responseTest = new byte[3];
		 
		bytes = stream.Read(rowData,0,rowData.Length);
		for(int i = 0; i < 3; i++)
		{
			responseTest[i] = rowData[i];
		}
		
		string responseString = encoding.GetString(responseTest);
				
		if(responseString == "OK\n" || responseString == "ER\n")
		{
			Debug.Log("Empatica reading stopped");
			dataChunk.Add(responseString);
			return dataChunk;
		} else {
			int numRows = System.BitConverter.ToInt32(rowData,0)/2;
			data = new byte[numRows * sizeof(float) * signalsEnabled];
			stream.Read(data,0,data.Length);
			
			for(int i = 0; i < numRows; i++)
			{
				float[] sv = new float[signalsAvailable];
				
				for(int j = 0; j < signalsEnabled; j++)
				{
					sv[j] = System.BitConverter.ToSingle(data,i * signalsEnabled * sizeof(float) + j * sizeof(float));
					
					for(int k = signalsAvailable-signalsEnabled; k < signalsAvailable; k++)
					{
						sv[k] = 0;
					}
				}
				
				EmpaticaSample sample;
				if(markWaiting)
				{
					sample = new EmpaticaSample(DateTime.Now,sv[0],sv[1],sv[2],sv[3],sv[4],sv[5],sv[6],sv[7],sv[8],waitingMark);
					markWaiting = false;
				} else {
					sample = new EmpaticaSample(DateTime.Now,sv[0],sv[1],sv[2],sv[3],sv[4],sv[5],sv[6],sv[7],sv[8],"");
				}
				dataChunk.Add(sample);
				}
				return dataChunk;
		}
	}
	#endregion
	
	#region Save to disk
	public string OutputFileName="Empatica_data.dat";
	public bool isSaving = false;
	public Thread saveThread;
	private void startSave()
	{
		saveThread = new Thread(new ThreadStart(saveDataToFile));
		saveThread.IsBackground = true;
		saveThread.Start();
	}
	private void saveDataToFile()
	{
		isSaving = true;
		
		string outString = "";
		
		Debug.Log("Building output string...");
		foreach(EmpaticaSample row in dataStorage)
		{
			outString += row.ToString();
			outString += "\n";
		}
		Debug.Log("Done building output string, writing to disk");
		
		System.IO.File.WriteAllText(OutputFileName,outString);
		
		Debug.Log("Write completed, aborting thread	");
		isSaving = false;
		
		saveThread.Abort();
	}
	#endregion
	
	#region GUI
	public void OnGUI()
	{
		if(showGUI)
		{
			if(GUI.Button(new Rect(10,10,100,100),"Connect()"))
			{
				ConnectEmpatica();
			}
			if(GUI.Button(new Rect(110,10,100,100),"Stop()")){
				StopEmpatica();
			}
			if(GUI.Button(new Rect(310,10,100,100),"Save Data")){
				startSave();
			}
			if(isSaving)
			{
				GUI.Box(new Rect(310,10,100,100),"Saving data...");
			}
		}
	}
	#endregion
}

#region Sample structure
//For storing data from the device
public class EmpaticaSample : System.Object
{
	public DateTime SampleTime;
	public float SkinConductance = 0;
	public float BloodVolumePulse = 0;
	public float Unknown1 = 0;
	public float Unknown2 = 0;
	public float Unknown3 = 0;
	public float Unknown4 = 0;
	public float Unknown5 = 0;
	public float Unknown6 = 0;
	public float Unknown7 = 0;
	public string Mark = "";
	
	public EmpaticaSample(DateTime st, float sc, float bvp, float u1, float u2,float u3, float u4,float u5,float u6,float u7, string mark)
	{
		SampleTime = st;
		SkinConductance = sc;
		BloodVolumePulse = bvp;
		Unknown1 = u1;
		Unknown2 = u2;
		Unknown3 = u3;
		Unknown4 = u4;
		Unknown5 = u5;
		Unknown6 = u6;
		Unknown7 = u7;
		Mark = mark;
	}
	
	public override string ToString ()
	{
		return string.Format ("{0:yyyy-mm-dd_hh:mm:ss.fff}\t{1:0.0000000000}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}",SampleTime,SkinConductance,BloodVolumePulse,Unknown1,Unknown2,Unknown3,Unknown4,Unknown5,Unknown6,Unknown7,Mark);
	}
}
#endregion