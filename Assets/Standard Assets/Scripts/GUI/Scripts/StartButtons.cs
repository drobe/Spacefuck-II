using UnityEngine;
using System.Collections;

public class StartButtons : CustomGUI
{
	[SerializeField]
	private string[] buttonLabels		= new string[2]{"Host Local","Host Network"};
	[SerializeField]
	private Vector2 offset				= new Vector2(0,0);
	
	public GUISkin skin 				= null;
	[SerializeField]
	private bool[] normalizedPOS		= new bool[2];
	[SerializeField]
	private bool[] inverseOrigin		= new bool[2];
	[SerializeField]
	private bool[] normalizedDimension	= new bool[2];
	public Rect rect;
	private Rect _rect;

	
	void OnGUI()
	{
		GUI.skin = skin;
		int[] d = new int[]{Screen.width,Screen.height};
		int[] center = new int[]{0,0};
		
		if (inverseOrigin[0])
		{
			center[0] = d[0];
		}
		if (inverseOrigin[1])
		{
			center[1] = d[1];
		}
		_rect = new Rect(center[0] + rect.x,center[1] + rect.y,rect.width,rect.height);
		
		if (normalizedPOS[0])
		{
			_rect.x = center[0] + (rect.x * d[0]);
		}
		if (normalizedPOS[1])
		{
			_rect.y = center[1] + (rect.y * d[1]);
		}
		
		if (normalizedDimension[0])
		{
			_rect.width = rect.width * d[0];
		}
		if (normalizedDimension[1])
		{
			_rect.height= rect.height * d[1];
		}
		
		_rect.x += offset.x;
		_rect.y += offset.y;
		
		Rect tempRect = _rect;
		tempRect.width *= 0.5f;
		
		if (GUI.Button(tempRect,buttonLabels[0]))
		{
			ModeManager.GoTo (GameMode.HostLocal);
		}
		
		tempRect.x += tempRect.width;
		
		if (GUI.Button(tempRect,buttonLabels[1]))
		{
			ModeManager.GoTo (GameMode.HostNetwork);
		}
	}
}
