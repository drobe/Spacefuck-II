using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkHostingScreen : MonoBehaviour 
{
	public GUISkin skin;
	public GameObject innerBounds;
	public Rect boxRect = new Rect();
	
	public Texture2D NullTexture;
	public Texture2D ControllerTexture;
	public Texture2D KeyboardTexture;
	public Texture2D NetworkTexture;
	public Texture2D AITexture;
	
	public Vector2 spacing 					= new Vector2(2,2);
	
	public float deviceUpdateCooldown 		= 1.0f;
	private float _deviceUpdateCooldown 	= 0f;
	
	List<InputDevice> devices;
	
	public static bool[] claimedSlot 		= new bool[4];
	
	public static bool areTheyReady			= false;
	public static bool autoServe			= false;
	
	private Vector2[] scrollPosition = new Vector2[4];
	[SerializeField]
	private float scrollBarWidth		= 6f; 
	
	void Start()
	{
		
	}
	
	
	void Update()
	{
		CalculateDimensions();
		UpdateDevices();
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
	
	private void UpdateDevices()
	{
		if (Time.realtimeSinceStartup > _deviceUpdateCooldown)
		{
			Spacefuck.InputManager.PlayerSlot[] slots = Spacefuck.InputManager.GetPlayerSlots();
			_deviceUpdateCooldown = Time.realtimeSinceStartup + deviceUpdateCooldown;
			for(int i = 0; i < slots.Length; i++)
			{
				if (!slots[i].device || slots[i].device.type != InputDeviceType.Network)
				{
					NetworkCommunicator.SendMessage(NetworkMessage.ServerDeviceClaim, i);
				}
				else if (slots[i].device.type == InputDeviceType.Network)
				{
					NetworkCommunicator.SendMessage(NetworkMessage.ServerDeviceUnClaim, i);
				}
			}
		}
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
				if (s.device == devices[i] && devices[i].type != InputDeviceType.Network && devices[i].type != InputDeviceType.AI)
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
		Rect GoButtonRect = new Rect(0,0,spacing.x * 4,spacing.y * 2);
		GoButtonRect.center = boxRect.center;
				
		if ((autoServe || GUI.Button(GoButtonRect,areTheyReady? "Go" : "Not Ready")) && areTheyReady)
		{
			for (int i = 0; i < claimedSlot.Length; i++)
			{
				if ((slots[i].device && slots[i].device.type == InputDeviceType.Network) && !claimedSlot[i])
				{
					Spacefuck.InputManager.SetDevice(i,null);
				}
			}
			
			ModeManager.GoTo (GameMode.Gameplay);
			NetworkCommunicator.SendMessage(NetworkMessage.Start, 0);
			GameplayController2.start = true;
		}
		
		Rect autoServeRect 	= GoButtonRect;
		autoServeRect.x 	= boxRect.x;
		autoServeRect.xMax	= autoServe? GoButtonRect.xMax : GoButtonRect.x;
		if (GUI.Button(autoServeRect, autoServe? "Disable AutoStart" : "Enable AutoStart"))
		{
			autoServe = ! autoServe;
		}
		
		GoButtonRect.x += GoButtonRect.width;
		GoButtonRect.width = dimensions.x - (spacing.x * 2);
		GUI.Box (GoButtonRect, NetworkCommunicator.numPlayers.ToString() + "/1 Connected");
	}
	
	void DrawPlayerBox(Rect r, int i, Spacefuck.InputManager.PlayerSlot p)
	{
		Vector2 boundaries 	= new Vector2(r.width, r.height);
		bool makeNetwork = false;
		
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
				case InputDeviceType.AI:
					myTex = AITexture;
					break;
				default:
					myTex = NullTexture;
					break;
				}
		}
		GUI.BeginGroup(r);
		Rect nameRect 			= new Rect(0,0,r.width * 0.35f,50);
		Rect netRect 			= new Rect(0,50,nameRect.width,50);
		GUI.Box(nameRect,"Player " + (i+1).ToString());
		if (!p.device || p.device.type != InputDeviceType.Network)
		{
			makeNetwork = GUI.Button(netRect,"+Net");
		}
		else
		{
			GUI.Box(netRect,"");
		}
		
		nameRect.x 				= nameRect.xMax;
		nameRect.width 			= r.width - nameRect.width;
		nameRect.height 		= 100;
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

				if (makeNetwork && d.type == InputDeviceType.Network)
				{
					Spacefuck.InputManager.SetDevice(i,d);
					NetworkCommunicator.SendMessage(NetworkMessage.ServerDeviceUnClaim, i);
					continue;
				}
				if (d == null || d.type == InputDeviceType.Network || (p.device && p.device.type == d.type))
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
				case InputDeviceType.AI:
					tex = AITexture;
					break;
				default:
					tex = NullTexture;
					break;
				}
				if (GUI.Button(buttonRect, new GUIContent(d.name,tex)))
				{
					Spacefuck.InputManager.SetDevice(i,d);
				}
				buttonRect.x += buttonRect.width;
				if (buttonRect.xMax > boundaries.x)
				{
					buttonRect.x = remainderX;
					buttonRect.y += buttonRect.height;
				}
			}
		}
		if (p.device != null && GUI.Button(buttonRect, "Clear"))
		{
			Spacefuck.InputManager.SetDevice(i,null);
			NetworkCommunicator.SendMessage(NetworkMessage.ServerDeviceClaim, i);
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