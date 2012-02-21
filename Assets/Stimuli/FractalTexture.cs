using UnityEngine;
using System.Collections;

public class FractalTexture : MonoBehaviour {
	/* Approripated from the public examples provided by Unity3d.com here:
	 * http://unity3d.com/support/resources/example-projects/procedural-examples.html
	 * */
	
	
	
	bool gray = true;
	int width = 128;
	int  height = 128;
	
	float lacunarity = 6.18F;
	float h = 0.69F;
	float octaves = 8.379F;
	float offset = 0.75F;
	float scale = 0.09F;
	
	float offsetPos = 0.0F;
	
	private Texture2D texture;
	private Perlin perlin;
	private FractalNoise fractal;
	
	void Start ()
	{
		texture = new Texture2D(width, height, TextureFormat.RGB24, false);
		renderer.material.mainTexture = texture;
	}
	
	void Update()
	{
		Calculate();
	}
	
	void Calculate()
	{
		if (perlin == null)
		{
			perlin = new Perlin();
		}
		
		fractal = new FractalNoise(h, lacunarity, octaves, perlin);
		
		for (int y = 0;y<height;y++)
		{
			for (int x = 0;x<width;x++)
			{
				if (gray)
				{
					float val = fractal.HybridMultifractal(x*scale + Time.time, y * scale + Time.time, offset);
					texture.SetPixel(x, y, new Color(val, val, val, val));
				}
				else
				{
					offsetPos = Time.time;
					float valuex = fractal.HybridMultifractal((float)(x*scale + offsetPos * 0.6), (float)(y*scale + offsetPos * 0.6), (float)offset);
					float valuey = fractal.HybridMultifractal((float)(x*scale + 161.7 + offsetPos * 0.2), (float)(y*scale + 161.7 + offsetPos * 0.3), (float)offset);
					float valuez = fractal.HybridMultifractal((float)(x*scale + 591.1 + offsetPos), (float)(y*scale + 591.1 + offsetPos * 0.1), (float)offset);
					texture.SetPixel(x, y, new Color (valuex, valuey, valuez, 1));
				}
			}	
		}
		
		texture.Apply();
	}
}
