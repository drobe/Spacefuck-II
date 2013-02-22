using UnityEngine;
using System.Collections;

public class Scanline : MonoBehaviour 
{
	public Texture2D scanlineTexture = null;
	
	public int skip					= 6;
	public int offset				= 0;
	public float linesToDraw		= 0;
	public int linesThisFrame		= 0;
	
	public int passes 				= 6;
	public float speed				= 1.0f;
	public int line					= 0;
	public int pass					= 0;
	
	public int[] dimensions			= new int[2];
	public int depth				= 0;
	public int thickness			= 1;
	private float LastTime			= 0;
	
	void Update()
	{	
		float deltaTime = Time.realtimeSinceStartup - LastTime;
		float x = Screen.height / speed;
		linesToDraw += deltaTime * x;
		
		if (linesToDraw >= 1.0f)
		{
			linesThisFrame = Mathf.FloorToInt(linesToDraw);
			linesToDraw -= linesThisFrame;
			
			line += skip;
			if (line > Screen.height)
			{
				offset += 1;
				line = offset;
			}
			if (offset >= skip)
			{
				offset = 0;
			}
		}
		LastTime = Time.realtimeSinceStartup;
	}
	
	void OnGUI()
	{
		int width = Screen.width;
		for(int i = 0; i < linesThisFrame; i++)
		{
			GUI.DrawTexture(new Rect(0,line + skip,width,thickness), scanlineTexture);
		}
	}
}