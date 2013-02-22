/* GameStart Class
 * A class that handles the startup operations on a new Game level.
 * 
 * In particular, this class handles the initialization of all asteriods and planetoids in a level.
 * It will also determine where players start.
 * This is particular to the regular verses mode.
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayController2 : InputManaged
{	
	public GameObject inner;
	public GameObject outer;
	
	public int minPlanetoids 				= 0;
	public int maxPlanetoids				= 3;
	
	public int minAsteroids 				= 0;
	public int maxAsteroids					= 100;
	
	static public bool start				= false;
	public bool reset 						= false;
	
	public static List<GameObject> allObjects	
											= new List<GameObject>();
	
	public GameMode mode 					= GameMode.Setup;
	public InputManaged[] 	playerObjects 	= new InputManaged[0];
	
	private GameObject[] asteroids;
	private GameObject[] planetoids;
	
	static private int livingPlayers				= -1;
	static private float endGameCooldown			= 5f;
	static private float _endGameCooldown			= 0;
	
	void Awake()
	{
	}
	
	void Start()
	{
		Spacefuck.InputManager.SetDefault(this);
		for (int i = 0; i < playerObjects.Length; i++)
		{
			Spacefuck.InputManager.SetPlayer(i,playerObjects[i]);
		}
	}
	
	void Begin()
	{
		ObjectPool.Clear();
		PlacePlayers();
		GeneratePlanetoids();
		GenerateAsteroids();
		//FixErrors();
		ClearPlayerStart();
		AIDevice.Begin();
	}
	
	void Update()
	{
		Spacefuck.InputManager.Update();
		
		if (reset || start)
		{
			Begin ();
			reset = false;
			start = false;
		}
		if (livingPlayers == 0 || livingPlayers == 1)
		{
			if (Time.realtimeSinceStartup > _endGameCooldown)
			{
				AIDevice.End();
				NetworkCommunicator.SendMessage(NetworkMessage.End,0);
				livingPlayers = -1;
				
				if (Network.isServer)
				{
					ModeManager.GoTo (GameMode.HostNetwork);
				}
				else if (Network.isClient)
				{
					ModeManager.GoTo (GameMode.JoinNetwork);
				}
				else
				{
					ModeManager.GoTo (GameMode.HostLocal);
				}
			}
		}
	}
	
	void FixedUpdate()
	{

	}
	
	private void GeneratePlanetoids()
	{
		int numPlanetoids = Random.Range(minPlanetoids, (maxPlanetoids + 1));
		planetoids = new GameObject[numPlanetoids];
		Bounds b = inner.collider.bounds;
		for(int i = 0; i < numPlanetoids; i++)
		{
			float x = Random.Range(b.min.x, b.max.x);
			float y = Random.Range(b.min.y, b.max.y);
			planetoids[i] = Planetoid.Instantiate(new Vector3(x,y,0),Quaternion.identity);
		}
	}
	private void GenerateAsteroids()
	{
		int numAsteroids = Random.Range(minAsteroids, (maxAsteroids + 1));
		asteroids = new GameObject[numAsteroids];
		Bounds b = inner.collider.bounds;
		for(int i = 0; i < numAsteroids; i++)
		{
			float x = Random.Range(b.min.x, b.max.x);
			float y = Random.Range(b.min.y, b.max.y);
			asteroids[i] = Asteroid.Instantiate(new Vector3(x,y,0),Quaternion.identity);
		}
	}
	private void PlacePlayers()
	{
		livingPlayers = 0;
		
		float radius = Mathf.Min(inner.collider.bounds.size.x, inner.collider.bounds.size.y);
		int numPlayers = 0;
		Spacefuck.InputManager.PlayerSlot[] playerslots = Spacefuck.InputManager.GetPlayerSlots();
		foreach (Spacefuck.InputManager.PlayerSlot p in playerslots)
		{
			if (p.device)
			{
				numPlayers++;
				livingPlayers++;
			}
		}
		float interval = (Mathf.PI * 2) / numPlayers;
		float position = Random.Range(-0.25f * Mathf.PI,0.25f * Mathf.PI);
		foreach (Spacefuck.InputManager.PlayerSlot p in playerslots)
		{
			if (p.device)
			{
				float x = Mathf.Cos(position);
				float y = Mathf.Sin(position);
				position += interval;
				p.managedObject.transform.position = new Vector3(x,y,0) * radius * Random.Range(0.25f, 0.45f);
				p.managedObject.gameObject.SetActive(true);
				p.managedObject.transform.Rotate(0,0,Random.Range(0,360));
			}
		}
	}
	private void FixErrors()
	{
		Bounds b = inner.collider.bounds;
		
		foreach (GameObject a in planetoids)
		{
			if (a)
			{
				Bounds c  = a.collider.bounds;
				if (!b.Contains(c.min) || !b.Contains(c.max))
				{
					Debug.DrawLine(c.min, c.max,Color.blue,1);
					
					ObjectPool.Destroy(a);
					continue;
				}
			}
		}
	}
	
	private void ClearPlayerStart()
	{
		foreach (InputManaged p in playerObjects)
		{
			Vector3 pp = p.collider.bounds.center;
			float ps = Mathf.Max(p.collider.bounds.size.x, p.collider.bounds.size.y);
			foreach (GameObject go in asteroids)
			{
				if (!go)
				{
					continue;
				}
				Vector3 gp = go.collider.bounds.center;
				float gs = Mathf.Max(p.collider.bounds.size.x, p.collider.bounds.size.y);
				if (Vector3.Distance(pp,gp) <= ps + gs)
				{
					ObjectPool.Destroy(go);
				}
			}
			foreach (GameObject go in planetoids)
			{
				if (!go)
				{
					continue;
				}
				Vector3 gp = go.collider.bounds.center;
				float gs = Mathf.Max(p.collider.bounds.size.x, p.collider.bounds.size.y);
				if (Vector3.Distance(pp,gp) <= ps + gs)
				{
					ObjectPool.Destroy(go);
				}
			}
		}
	}
	
	public static void PlayerHasDied()
	{
		livingPlayers--;
		_endGameCooldown = Time.realtimeSinceStartup + endGameCooldown;
	}
	
	void OnPlayerConnected(NetworkPlayer player) 
	{
        Debug.Log("Player connected from " + player.ipAddress + ":" + player.port);
    }
	
	public override void Send (CommandCode[] codes)
	{
//		foreach (CommandCode c in codes)
//		{
//			if (c == CommandCode.MenuAccept)
//			{
//				mode = GameMode.Gameplay;
//			}
//			else if (c == CommandCode.SoftQuit)
//			{
//				mode = GameMode.Setup;
//			}
//		}
	}
	
	void OnGUI()
	{
//		if (mode == GameMode.Gameplay)
//		{
//			if (GUI.Button(new Rect(5,5,200,50),"Reset"))
//			{
//				reset = true;
//			}
//			if (GUI.Button(new Rect(5,55,200,50),"Start Server"))
//			{
//				Network.InitializeServer(128,6666,false);
//			}
//			if (GUI.Button(new Rect(5,105,200,50),"Connect"))
//			{
//				Network.Connect("192.168.1.128",6666);
//			}
//		}
//		else if (mode == GameMode.Setup)
//		{
//			Spacefuck.InputManager.PlayerSlot[] slots = Spacefuck.InputManager.GetPlayerSlots();
//			InputDevice[] devices = Spacefuck.InputManager.GetDevices();
//			
//			Rect boxRect = new Rect(0,0,100,devices.Length * 20);
//			
//			
//			for(int i = 0; i < slots.Length; i++)
//			{
//				Spacefuck.InputManager.PlayerSlot s = slots[i];
//				string ss = "Player " + i.ToString() 
//							+ "\n>> " + (s.device? s.device.name : "Empty")
//							+ "\n>> " + (s.managedObject? s.managedObject.name : "Empty");
//				
//				GUI.Box(boxRect, ss);
//				
//				Rect deviceRect = boxRect;
//				deviceRect.x += boxRect.width;
//				deviceRect.height = 20;
//				
//				foreach (InputDevice d in devices)
//				{
//					if (GUI.Button(deviceRect, d.name))
//					{
//						Spacefuck.InputManager.SetDevice(i,d);
//					}
//					deviceRect.y += deviceRect.height;
//				}
//				boxRect.y += boxRect.height + 20;
//			}
//		}
	}
}
