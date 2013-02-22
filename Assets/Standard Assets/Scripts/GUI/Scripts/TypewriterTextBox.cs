using UnityEngine;
using System.Collections;

public class TypewriterTextBox : CustomGUI
{
	static private TypewriterTextBox 	instance;
	
	private float nextType 				= 0;
	public float typeDelay 				= 0.25f;
	public float lineDelay				= 0.5f;
	
	[SerializeField]
	private string typeString 			= "";
	
	[SerializeField]
	private string OutString			= "";
	
	[SerializeField]
	private string preString			= "";
	
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
	
	public bool _isActive				= false;
	public bool isActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			_isActive = value;
		}
	}
	void Awake()
	{
		instance = this;
	}
	public void Start()
	{
		OutString = preString;
	}
	
	public void Clear()
	{
		typeString = "";
		OutString = preString;
	}
	
	
	static public void Add(string s)
	{
		instance.typeString += s;
	}
	
	static public void Add_Inturrupt(string s)
	{
		instance.typeString = "^"+s;
	}

	void _Update () 
	{
		if (_isActive)
		{
			if (Time.realtimeSinceStartup > nextType)
			{
				nextType = Time.realtimeSinceStartup + typeDelay;
				if (typeString.Length > 0)
				{
					char nextChar = typeString[0];
					typeString = typeString.Remove(0,1);
					if (nextChar == '\n' || nextChar == '\r' || nextChar == '^')
					{
						OutString += "\n" + preString;
						nextType = Time.realtimeSinceStartup + lineDelay;
					}
					else
					{
						OutString += nextChar;
						nextType = Time.realtimeSinceStartup + typeDelay;
					}
				}
				else
				{
					nextType = Time.realtimeSinceStartup + typeDelay;
				}
			}
		}
	}
	
	static public void External_Update()
	{
		instance._Update();
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
		
		GUI.Box(_rect,OutString);
	}
}

