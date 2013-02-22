using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkJoinScreen : MonoBehaviour 
{
	public GUISkin skin;
	public GameObject innerBounds;
	public Rect boxRect = new Rect();
	
	public Texture2D NullTexture;
	public Texture2D ControllerTexture;
	public Texture2D KeyboardTexture;
	public Texture2D NetworkTexture;
	
	public Vector2 spacing 	= new Vector2(2,2);
	
	static public bool amIready	= false;
	
	List<InputDevice> devices;
	
	private Vector2[] scrollPosition = new Vector2[4];
	[SerializeField]
	private float scrollBarWidth		= 6f; 
	
	void Start()
	{
		
	}
	
	
	void Update()
	{
		CalculateDimensions();
	}
	
	private void CalculateDimensions()
	{
		Vector3[] positions = new Vector3[2];
		Vector3 invertedMin = innerBounds.collider.bounds.min;
		invertedMin.y *= -1;
		invertedMin.z = 0;
		Vector3 invertedMax = innerBounds.collider.bounds.max;
		invertedMax.y *= -1;
		invertedMax.z = 0;
		positions[0] = Camera.mainCamera.WorldToScreenPoint(invertedMin);
		positions[1] = Camera.mainCamera.WorldToScreenPoint(invertedMax);
		
		boxRect = new Rect(0,0,0,0);
		boxRect.x = positions[0].x;
		boxRect.y = Screen.height - positions[0].y;
		boxRect.width = positions[1].x - positions[0].x;
		boxRect.height =  positions[0].y - positions[1].y;
	}
	
	void OnGUI()
	{				
		GUI.skin = skin;

		devices = new List<InputDevice>(Spacefuck.InputManager.GetDevices());
		devices.Sort(SortDevices);
		Spacefuck.InputManager.PlayerSlot[] slots = Spacefuck.InputManager.GetPlayerSlots();
		
		foreach(Spacefuck.InputManager.PlayerSlot s in slots)
		{
			for(int i = 0; i < devices.Count; i++)
			{
				if (s.device == devices[i])
				{
					devices.RemoveAt(i);
					break;
				}
			}
		}

		
		int[] grid = new int[2];
		grid[0] = Mathf.CeilToInt(Mathf.Pow(slots.Length, 0.5f));
		grid[1] = Mathf.CeilToInt(Mathf.Pow(slots.Length, 0.5f));
		Vector2 dimensions = new Vector2(
			(boxRect.width / grid[0]),
			(boxRect.height / grid[1]));
		
		Rect GoButtonRect = new Rect(0,0,spacing.x * 4,spacing.y * 2);
		GoButtonRect.center = boxRect.center;
		if (GUI.Button(GoButtonRect,amIready? "Not Ready" : "Ready"))
		{
			if (amIready)
			{
				NetworkCommunicator.SendMessage(NetworkMessage.NotReady, 0);
			}
			else
			{
				NetworkCommunicator.SendMessage(NetworkMessage.Ready, 0);
			}
			amIready = ! amIready;
		}
		
		
		int count = 0;
		for(int y = 0; y < grid[0]; y++)
		{
			for(int x = 0; x < grid[1]; x++)
			{
				if (count >= slots.Length)
				{
					break;
				}
				Rect playerRect = new Rect(
					boxRect.x + (dimensions.x * x) + (x * spacing.x),
					boxRect.y + (dimensions.y * y) + (y * spacing.y),
					dimensions.x - (spacing.x),
					dimensions.y - (spacing.y));
				
				DrawPlayerBox(playerRect, count, slots[count]);
				count++;
			}
		}
	}
	
	void DrawPlayerBox(Rect r, int i, Spacefuck.InputManager.PlayerSlot p)
	{
		Vector2 boundaries 	= new Vector2(r.width, r.height);
		
		GUI.Box(r,"");
		
		Texture2D myTex;
		string dName;
		if (p.device == null)
		{
			myTex = NullTexture;
			dName = "Empty";
		}
		else
		{
			dName = p.device.name;
			switch(p.device.type)
				{
				case InputDeviceType.Controller:
					myTex = ControllerTexture;
					break;
				case InputDeviceType.Keyboard:
					myTex = KeyboardTexture;
					break;
			case InputDeviceType.Network:
					myTex = NetworkTexture;
					break;
				default:
					myTex = NullTexture;
					break;
				}
		}
		GUI.BeginGroup(r);
		Rect nameRect 			= new Rect(0,0,r.width * 0.35f,100);
		GUI.Box(nameRect,"Player " + (i+1).ToString());
		nameRect.x 				= nameRect.xMax;
		nameRect.width 			= r.width - nameRect.width;
		GUI.Box(nameRect,new GUIContent(dName,myTex));
		
		int remainderX 		= Mathf.RoundToInt((r.width % 100) * 0.5f);
		int remainderY		= Mathf.RoundToInt((r.height % 100) * 0.5f);
		Rect buttonRect 	= new Rect(remainderX,nameRect.height + remainderY,100,100);
		
		int rows			= Mathf.CeilToInt (devices.Count / Mathf.FloorToInt(r.width / buttonRect.width)); 
		Rect scrollRect		= nameRect;
		scrollRect.x		= 0;
		scrollRect.width	= r.width;
		scrollRect.y		= nameRect.yMax;
		scrollRect.height	= r.height - nameRect.height;
		Rect viewRect 		= new Rect(0,0,0,0);
		viewRect.width 		-= (2*scrollBarWidth);
		viewRect.height		= Mathf.Max(scrollRect.height, rows * (buttonRect.height) + (spacing.y * (rows -1)));
		
		scrollPosition[i] 	= GUI.BeginScrollView(scrollRect,scrollPosition[i],viewRect,false, true);
		
		buttonRect.y 		= remainderY;

		if (p.device && p.device.type == InputDeviceType.Network)
		{
		}
		else
		{
			foreach (InputDevice d in devices)
			{
				if (d == null || d.type == InputDeviceType.Network || d.type == InputDeviceType.AI)
				{
					continue;
				}
				Texture2D tex;
				
				switch(d.type)
				{
				case InputDeviceType.Controller:
					tex = ControllerTexture;
					break;
				case InputDeviceType.Keyboard:
					tex = KeyboardTexture;
					break;
				default:
					tex = NullTexture;
					break;
				}
				if (GUI.Button(buttonRect, new GUIContent(d.name,tex)))
				{
					Spacefuck.InputManager.SetDevice(i,d);
					NetworkCommunicator.SendMessage(NetworkMessage.ClientDeviceClaim, i);
				}
				buttonRect.x += buttonRect.width;
				if (buttonRect.xMax > boundaries.x)
				{
					buttonRect.x = remainderX;
					buttonRect.y += buttonRect.height;
				}
			}
			if (p.device != null && GUI.Button(buttonRect, "Clear"))
			{
				Spacefuck.InputManager.SetDevice(i,null);
				NetworkCommunicator.SendMessage(NetworkMessage.ClientDeviceUnClaim, i);
			}
		}
		GUI.EndScrollView();
		GUI.EndGroup();
	}
	
	private static int SortDevices(InputDevice a, InputDevice b)
	{
		if (a.type == b.type)
		{
			return string.Compare(a.name, b.name, true);
		}
		return ((int) a.type > (int) b.type)? 1 : -1;
	}
}