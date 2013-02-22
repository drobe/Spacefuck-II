using UnityEngine;
using System.Collections;

public class DirectConnectButton : CustomGUI
{
	public string preString 			= ">";
	public string blinkString 			= "|";
	
	private bool blink					= false;
	private float nextBlink 			= 0;
	public float blinkTime 				= 0.25f;
	
	public string typeString 			= "";
	
	[SerializeField]
	private bool acceptNewLine			= false;
	private string OutString			= "";
	
	[SerializeField]
	private bool title					= true;
	[SerializeField]
	private float titleHeight			= 20f;
	[SerializeField]
	private string titleLabel			= "Server List";
	
	[SerializeField]
	private float buttonScale			= 0.25f;
	[SerializeField]
	private float buttonBuffer			= 16f;
	[SerializeField]
	private string buttonLabel			= "Submit";
	
	[SerializeField]
	private Vector2 offset				= new Vector2(0,0);
	
	public GUISkin skin 				= null;
	[SerializeField]
	private bool[] normalizedPOS		= new bool[2];
	[SerializeField]
	private bool[] inverseOrigin		= new bool[2];
	[SerializeField]
	private bool[] normalizedDimension	= new bool[2];
	public Rect rect					= new Rect(0,0,0,0);
	private Rect _rect					= new Rect(0,0,0,0);
	
	private bool _isActive				= false;
	public bool isActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			_isActive = value;
			blink = false;
			OutString = preString + typeString;
		}
	}
	
	void Start()
	{
		OutString = preString + typeString + (blink? blinkString : "");
	}

	void Update () 
	{
		if (_isActive)
		{
			if (Time.realtimeSinceStartup > nextBlink)
			{
				blink = !blink;
				nextBlink = Time.realtimeSinceStartup + blinkTime;
				OutString = preString + typeString + (blink? blinkString : "");
			}
			
			string inputString = Input.inputString;
			if (inputString.Length > 0)
			{
				foreach (char c in inputString)
				{
					if (c == '\b' && typeString.Length > 0)
					{
						typeString = typeString.Remove(typeString.Length -1);
					}
					else if (c == '\n' || c == '\r' && acceptNewLine)
					{
						typeString += "\n" + preString;
					}
					else
					{
						typeString += c;
					}
				}
				OutString = preString + typeString + (blink? blinkString : "");
			}
			
		}
	}
	
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
		
		if (title)
		{
			Rect titleRect = _rect;
			titleRect.height = titleHeight;
			GUI.Label(titleRect,titleLabel);
			
			_rect.y 			+= titleHeight;
			_rect.height 		-= titleHeight;
		}
		
		Rect tempRect = _rect;
		tempRect.width = (tempRect.width * (1.0f - buttonScale)) - buttonBuffer;

		if (GUI.Button(tempRect, OutString))
		{
			isActive = true;
		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				isActive = false;
			}
		}
		
		tempRect 	= _rect;
		tempRect.x 	+= (tempRect.width * (1.0f - buttonScale)) - buttonBuffer;
		tempRect.width = (tempRect.width * buttonScale) + buttonBuffer;
		
		if (GUI.Button (tempRect, buttonLabel))
		{
			if (typeString.Length == 0 || !typeString.Contains("."))
			{
			}
			else
			{
				Network.Connect(typeString,6666);
				Spacefuck.InputManager.NetPrep();
				TypewriterTextBox.Add ("^^---^Connect: " + typeString);
			}
		}
	}
}
