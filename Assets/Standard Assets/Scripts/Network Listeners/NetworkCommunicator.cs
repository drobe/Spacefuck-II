using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkCommunicator : MonoBehaviour 
{
	public Queue<string> DEBUG_STRING_LIST 		= new Queue<string>();
	public float DEBUG_SIZE						= 20;
	public float DEBUG_TRIM						= 20;
	static public int numPlayers				= 0;
	
	/****************************************************************/
	// Members
	/*********************/
	static private NetworkCommunicator instance;
	static private int 		InstanceCount 	= 0;
	
	static private List<int> 	activeList	;
	static private bool[] 	updateValidity	;
	static private Queue<int>	updateQueue	;
	
	[SerializeField]
	private float 	delay 					= 0.1f;
	private float 	lastUpdate				= 0;
	private bool 	doBatch					= false;
	private int index						= 0;
	/****************************************************************/
	
	/****************************************************************/
	// Constructors and Deconstructors
	/*********************/
	void Awake()
	{
		if (InstanceCount >= 1 || InstanceCount < 0)
		{
			Debug.LogWarning(InstanceCount.ToString() + " NetworkCommunicator instances.");
		}
		instance 	= this;
		InstanceCount ++;
	}
	
	void Start()
	{	
		lastUpdate 				= Time.realtimeSinceStartup;
		updateValidity			= new bool[transform.childCount];
		activeList 				= new List<int>(transform.childCount);
		updateQueue				= new Queue<int>(transform.childCount);
		
//		for(int i = 0; i < transform.childCount; i++)
//		{
//			GameObject go 		= transform.GetChild(i).gameObject;
//			ChangeDetector2 cd 	= go.GetComponent<ChangeDetector2>();
//			if (cd)
//			{
//				cd.SetIndex(i);
//			}
//		}
	}

	void OnDestroy()
	{
		instance = null;
		InstanceCount --;
		if (InstanceCount == 0)
		{
			activeList.Clear();
			updateQueue.Clear();
			updateValidity 		= new bool[0];
			lastUpdate 			= 0;
		}
	}
	/****************************************************************/
	
	/****************************************************************/
	// Updates
	/*********************/
	void Update()
	{
		
		if (Time.realtimeSinceStartup > lastUpdate)
		{
			if (Network.isServer)
			{
				if (activeList.Count > 0)
				{
//					SendToClients(activeList[Random.Range(0,activeList.Count)]);
					if (index >= activeList.Count)
					{
						index = 0;
					}
					SendToClients(activeList[index]);
					index++;
				}

				lastUpdate = Time.realtimeSinceStartup + delay;
				for(int i = 0; i < ObjectPool.GetStatic(); i++)
				{
					Queue(i);
				}
			}
		}
	}
	
	void FixedUpdate()
	{
		if (Network.isServer)
		{
			if (doBatch)
			{
				DEBUG_STRING_LIST.Enqueue("Batch out: " + updateQueue.Count.ToString());
				Batch ();
				doBatch = false;
			}
			else if(updateQueue.Count > 0)
			{
				doBatch = true;
			}
		}
	}
	/****************************************************************/
	
	/****************************************************************/
	// Activate and Deactivate notifiers for network managed objects.
	// Objects will mark themselves as active using these functions.
	// This is necessary for telling the Network Communicator to 
	// dither updates for these objects.
	//
	// These functions will also activate and deactivate remote objects
	// on network clients.
	/*********************/
	static public void Activate(int i)
	{
		activeList.Add(i);
		if (Network.isServer)
		{
//			GameObject go 					= ObjectPool.Get(i);
			instance.networkView.RPC("NetActivate", RPCMode.Others,i);
			
//			instance.networkView.RPC("NetActivate", RPCMode.Others, 
//				go.transform.position, 
//				go.rigidbody.velocity, 
//				go.rigidbody.angularVelocity,
//				go.transform.localScale,
//				go.transform.rotation,
//				i);
			
			NetworkCommunicator.Queue(i);
		}
	}
	
	static public void Deactivate(int i)
	{
		activeList.Remove(i);
		if (Network.isServer)
		{
			instance.networkView.RPC("NetDeactivate", RPCMode.Others,i);
		}
	}
	
//	[RPC]
//	void NetActivate(Vector3 p, Vector3 v, Vector3 a, Vector3 l, Quaternion r, int i)
//	{
//		if (Network.isClient)
//		{
//			GameObject go 					= ObjectPool.Get (i);
//			go.transform.position 			= p;
//			go.transform.rotation 			= r;
//			go.transform.localScale 		= l;
//			go.rigidbody.velocity			= v;
//			go.rigidbody.angularVelocity 	= a;
//			go.SetActive(true);
//		}
//	}
	
	[RPC]
	void NetActivate(int i)
	{
		if (Network.isClient)
		{
			GameObject go 					= ObjectPool.Get (i);
			go.SetActive(true);
		}
		DEBUG_STRING_LIST.Enqueue("Activate: " + i.ToString());
	}
	
	[RPC]
	void NetDeactivate(int i)
	{
		if (Network.isClient)
		{
			ObjectPool.Get (i).gameObject.SetActive(false);
		}
		DEBUG_STRING_LIST.Enqueue("Deactivate: " + i.ToString());
	}
	/****************************************************************/
	
	/****************************************************************/
	// A message sending structure for server-client communication not otherwise specified.
	static public void SendMessage(NetworkMessage message, int contents)
	{
		if (Network.isClient || Network.isServer)
		{
			instance.networkView.RPC("GetMessage", RPCMode.Others, (int) message, contents);
		}
	}
	
	// A message recieving structure for server-client communication not otherwise specified.
	[RPC]
	public void GetMessage(int message, int contents)
	{
		NetworkMessage nm = (NetworkMessage) message;
		
		switch(nm)
		{
		case NetworkMessage.Start:
			ModeManager.GoTo(GameMode.Gameplay);
			break;
		case NetworkMessage.End:
			ModeManager.GoTo(GameMode.JoinNetwork);
			break;
		case NetworkMessage.ServerDeviceClaim:
			Spacefuck.InputManager.SetDevice(contents,NetworkDevice.device);
			break;
		case NetworkMessage.ServerDeviceUnClaim:
			Spacefuck.InputManager.PlayerSlot p = Spacefuck.InputManager.GetPlayerSlots()[contents];
			if (p.device && p.device.type == InputDeviceType.Network)
			{
				Spacefuck.InputManager.SetDevice(contents,null);
			}
			break;
		case NetworkMessage.Quit:
			TypewriterTextBox.Add("^Server disconnected.");
			Network.Disconnect(10);
			break;
		case NetworkMessage.ClientDeviceClaim:
			NetworkHostingScreen.claimedSlot[contents] = true;
			break;
		case NetworkMessage.ClientDeviceUnClaim:
			NetworkHostingScreen.claimedSlot[contents] = false;
			break;
		case NetworkMessage.Ready:
			NetworkHostingScreen.areTheyReady = true;
			break;
		case NetworkMessage.NotReady:
			NetworkHostingScreen.areTheyReady = false;
			break;
		}
	}
	/****************************************************************/
	
	[RPC]
	void Synchronize(Vector3 p, Vector3 v, Vector3 a, Vector3 l, Quaternion r, float m, int i)
	{
		GameObject go = ObjectPool.Get (i).gameObject;
		go.transform.position 			= p;
		go.transform.rotation 			= r;
		go.transform.localScale 		= l;
		go.rigidbody.velocity			= v;
		go.rigidbody.angularVelocity 	= a;
		go.rigidbody.mass				= m;
		
		DEBUG_STRING_LIST.Enqueue("Synch: " + i.ToString());
	}
	
	[RPC]
	void BatchSynchronize(byte[] p, byte[] v, byte[] a, byte[] l, byte[] r, byte[] m, byte[] i)
	{
		DEBUG_STRING_LIST.Enqueue("Batch Start");
		string errorString = "";
		if (p.Length != v.Length || p.Length != l.Length)
		{
			errorString += "Bad BatchSynchronize; Inequal Length VectorStrings.";
		}
		if (i.Length % 2 != 0 || p.Length % 6 != 0 || v.Length % 6 != 0 || l.Length % 6 != 0)
		{
			errorString += "Bad BatchSynchronize; Incorrect VectorString Lengths.";
		}
		if (errorString.Length > 0)
		{
			Debug.LogWarning(errorString, this);
			return;
		}
		
		int[] indexArray;
		Vector3[] positionArray;
		Vector3[] velocityArray;
		Vector3[] angularVelocityArray;
		Vector3[] scaleArray;
		float[] massArray;
		Quaternion[] rotationArray;
		
		byte[] byteArray;
		
		int numIndices = i.Length / 4;
		
		// Read Indices
		indexArray= new int[numIndices];
		byteArray = i;

		for (int j = 0; j < numIndices; j++)
		{
			indexArray[j] = System.BitConverter.ToInt32(byteArray, 4 * j);
		}
		
		// Read positions
		positionArray= new Vector3[indexArray.Length];
		byteArray = p;

		for (int j = 0; j < numIndices; j++)
		{
			Vector3 position = Vector3.zero;
			position.x = System.BitConverter.ToSingle(byteArray, (12 * j) + 0);
			position.y = System.BitConverter.ToSingle(byteArray, (12 * j) + 4);
			position.z = System.BitConverter.ToSingle(byteArray, (12 * j) + 8);
			positionArray[j] = position;
		}
		
		// Read Velocities
		velocityArray= new Vector3[indexArray.Length];
		byteArray = v;

		for (int j = 0; j < numIndices; j++)
		{
			Vector3 velocity = Vector3.zero;
			velocity.x = System.BitConverter.ToSingle(byteArray, (12 * j) + 0);
			velocity.y = System.BitConverter.ToSingle(byteArray, (12 * j) + 4);
			velocity.z = System.BitConverter.ToSingle(byteArray, (12 * j) + 8);
			velocityArray[j] = velocity;
		}
		
		// Read Angular Velocities
		angularVelocityArray= new Vector3[indexArray.Length];
		byteArray = a;

		for (int j = 0; j < numIndices; j++)
		{
			Vector3 aVelocity = Vector3.zero;
			aVelocity.x = System.BitConverter.ToSingle(byteArray, (12 * j) + 0);
			aVelocity.y = System.BitConverter.ToSingle(byteArray, (12 * j) + 4);
			aVelocity.z = System.BitConverter.ToSingle(byteArray, (12 * j) + 8);
			angularVelocityArray[j] = aVelocity;
		}
		
		// Read Scales
		scaleArray= new Vector3[indexArray.Length];
		byteArray = l;

		for (int j = 0; j < numIndices; j++)
		{
			Vector3 scale = Vector3.zero;
			scale.x = System.BitConverter.ToSingle(byteArray, (12 * j) + 0);
			scale.y = System.BitConverter.ToSingle(byteArray, (12 * j) + 4);
			scale.z = System.BitConverter.ToSingle(byteArray, (12 * j) + 8);
			scaleArray[j] = scale;
		}
		
		// Read Rotations
		rotationArray= new Quaternion[indexArray.Length];
		byteArray = r;

		for (int j = 0; j < numIndices; j++)
		{
			Quaternion rotation = Quaternion.identity;
			rotation.x = System.BitConverter.ToSingle(byteArray, (16 * j) + 0);
			rotation.y = System.BitConverter.ToSingle(byteArray, (16 * j) + 4);
			rotation.z = System.BitConverter.ToSingle(byteArray, (16 * j) + 8);
			rotation.w = System.BitConverter.ToSingle(byteArray, (16 * j) + 12);
			rotationArray[j] = rotation;
		}
		
		// Read Masses
		massArray= new float[indexArray.Length];
		byteArray = m;

		for (int j = 0; j < numIndices; j++)
		{
			massArray[j] = System.BitConverter.ToSingle(byteArray, (4 * j));
		}
		
		// Apply changes
		int count = 0;
		foreach (int j in indexArray)
		{
			GameObject go 					= ObjectPool.Get (j);
			go.transform.position 			= positionArray[count];
			go.transform.localScale 		= scaleArray[count];
			go.rigidbody.velocity 			= velocityArray[count];
			go.rigidbody.angularVelocity 	= angularVelocityArray[count];
			go.transform.rotation 			= rotationArray[count];
			go.rigidbody.mass				= massArray[count];
			count++;
		}
		DEBUG_STRING_LIST.Enqueue("Batch: > " + numIndices.ToString());
	}
	
	private byte[] ReadBytes(int size,char[] charArray)
	{
		byte[] byteArray = new byte[size];
		int byteIndex = 0;
		foreach(char c in charArray)
		{
			byte[] bytes = System.BitConverter.GetBytes(c);
			foreach (byte b in bytes)
			{
				byteArray[byteIndex] = b;
				byteIndex++;
			}
		}
		return byteArray;
	}
	
	private void Batch()
	{
		List<byte> p = new List<byte>(updateQueue.Count * 6);
		List<byte> v = new List<byte>(updateQueue.Count * 6);
		List<byte> a = new List<byte>(updateQueue.Count * 6);
		List<byte> l = new List<byte>(updateQueue.Count * 6);
		List<byte> r = new List<byte>(updateQueue.Count * 8);
		List<byte> m = new List<byte>(updateQueue.Count * 4);
		List<byte> i = new List<byte>(updateQueue.Count * 4);
		
		
		while (updateQueue.Count > 0)
		{
			int indexI 				= updateQueue.Dequeue();
			GameObject go 			= ObjectPool.Get (indexI);
			updateValidity[indexI] 	= false;
			
			p.AddRange(System.BitConverter.GetBytes(go.transform.position.x));
			p.AddRange(System.BitConverter.GetBytes(go.transform.position.y));
			p.AddRange(System.BitConverter.GetBytes(go.transform.position.z));
			
			v.AddRange(System.BitConverter.GetBytes(go.rigidbody.velocity.x));
			v.AddRange(System.BitConverter.GetBytes(go.rigidbody.velocity.y));
			v.AddRange(System.BitConverter.GetBytes(go.rigidbody.velocity.z));
			
			a.AddRange(System.BitConverter.GetBytes(go.rigidbody.angularVelocity.x));
			a.AddRange(System.BitConverter.GetBytes(go.rigidbody.angularVelocity.y));
			a.AddRange(System.BitConverter.GetBytes(go.rigidbody.angularVelocity.z));
			
			l.AddRange(System.BitConverter.GetBytes(go.transform.localScale.x));
			l.AddRange(System.BitConverter.GetBytes(go.transform.localScale.y));
			l.AddRange(System.BitConverter.GetBytes(go.transform.localScale.z));
			
			r.AddRange(System.BitConverter.GetBytes(go.rigidbody.rotation.x));
			r.AddRange(System.BitConverter.GetBytes(go.rigidbody.rotation.y));
			r.AddRange(System.BitConverter.GetBytes(go.rigidbody.rotation.z));
			r.AddRange(System.BitConverter.GetBytes(go.rigidbody.rotation.w));
			
			m.AddRange(System.BitConverter.GetBytes(go.rigidbody.mass));
			
			i.AddRange(System.BitConverter.GetBytes(indexI));
		}		
		
		networkView.RPC("BatchSynchronize",RPCMode.Others,
			p.ToArray(),
			v.ToArray(),
			a.ToArray(),
			l.ToArray(),
			r.ToArray(),
			m.ToArray(),
			i.ToArray());
	}
	
	public static void Queue(int i)
	{
		if (i < 0 || i > updateValidity.Length)
		{
			print (i.ToString() + " vs: " + updateValidity.Length.ToString());
		}
		if (!updateValidity[i])
		{
			updateQueue.Enqueue(i);
		}
		updateValidity[i] = true;
	}
	
	private void SendToClients(int i)
	{
		if (Network.isServer)
		{
			//print (i.ToString() + " of " + updateValidity.Length.ToString());
			updateValidity[i] 	= false;
			GameObject go 		= ObjectPool.Get (i);
			networkView.RPC("Synchronize", RPCMode.Others, 
				go.transform.position, 
				go.rigidbody.velocity, 
				go.rigidbody.angularVelocity,
				go.transform.localScale,
				go.transform.rotation,
				go.rigidbody.mass,
				i);
		}
	}
	
	static public void SendCommand(int i, CommandCode[] commands)
	{
		if (commands.Length == 0)
		{
			return;
		}

		byte[] outCommands = new byte[(commands.Length * 4) + 4];
		
		byte[] bytes = System.BitConverter.GetBytes(i);
		for(int j = 0; j < bytes.Length; j++)
		{
			outCommands[j] = bytes[j];
		}

		for(int c = 0; c < commands.Length; c++)
		{
			bytes = System.BitConverter.GetBytes((int)commands[c]);
			for(int j = 0; j < bytes.Length; j++)
			{
				outCommands[(4 * c) + j + 4] = bytes[j];
			}
		}
		instance.networkView.RPC("GetCommand", RPCMode.Server, outCommands);
	}
	
	[RPC]
	void GetCommand(byte[] commands)
	{
		CommandCode[] commandCodes;
		int numCommands = (commands.Length / 4) - 1;
		int player = System.BitConverter.ToInt32(commands, 0);
		
		// Read Commands
		commandCodes = new CommandCode[numCommands];

		for (int i = 0; i < numCommands; i++)
		{
			commandCodes[i] = (CommandCode) System.BitConverter.ToInt32(commands, (4 * i) + 4);
		}
		
		Spacefuck.InputManager.DirectSend(player, commandCodes);
		
		DEBUG_STRING_LIST.Enqueue("P: " + player.ToString() + " > " + commandCodes.ToString());
	}
	
	void OnGUI()
	{
//		while (DEBUG_STRING_LIST.Count > Mathf.RoundToInt(DEBUG_TRIM))
//		{
//			DEBUG_STRING_LIST.Dequeue();
//		}
//		
//		Rect boxRect = new Rect(Screen.width -100,0,100,DEBUG_SIZE);
//		
//		foreach (string s in DEBUG_STRING_LIST)
//		{
//			GUI.Label(boxRect,s);
//			boxRect.y += DEBUG_SIZE;
//		}
//		boxRect = new Rect(Screen.width -200,0,100,DEBUG_SIZE);
//		delay = GUI.HorizontalSlider(boxRect,delay,0.01f,1.0f);
//		boxRect.y += DEBUG_SIZE;
//		GUI.Label(boxRect,"Delay: " + delay.ToString());
//		
//		boxRect.y += DEBUG_SIZE;
//		
//		DEBUG_SIZE = GUI.HorizontalSlider(boxRect,DEBUG_SIZE,1f,30f);
//		boxRect.y += DEBUG_SIZE;
//		GUI.Label(boxRect,"Size: " + DEBUG_SIZE.ToString());
//		
//		boxRect.y += DEBUG_SIZE;
//		DEBUG_TRIM = GUI.HorizontalSlider(boxRect,DEBUG_TRIM,1f,40f);
//		boxRect.y += DEBUG_SIZE;
//		GUI.Label(boxRect,"Trim: " + DEBUG_TRIM.ToString());
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		TypewriterTextBox.Add ("^Connection from: " + player.ipAddress);
		numPlayers++;
		NetworkHostingScreen.claimedSlot = new bool[4];
		NetworkHostingScreen.areTheyReady = false;
	}
	void OnServerInitialized()
	{
		TypewriterTextBox.Add ("^^---^Initialized Server.");
		ModeManager.GoTo (GameMode.HostNetwork);
		NetworkHostingScreen.claimedSlot = new bool[4];
		NetworkHostingScreen.areTheyReady = false;
	}	
	void OnConnectedToServer()
	{
		TypewriterTextBox.Add ("^Connected to server.");
		ModeManager.GoTo (GameMode.JoinNetwork);
	}
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		TypewriterTextBox.Add ("^" + player.ipAddress + " disconnected.");
		numPlayers--;
		NetworkHostingScreen.claimedSlot = new bool[4];
		NetworkHostingScreen.areTheyReady = false;
	}
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		TypewriterTextBox.Add ("^Disconnected: " + info.ToString() + "^---");
		ModeManager.GoTo(GameMode.MainMenu);
	}
	void OnFailedToConnect(NetworkConnectionError error)
	{
		TypewriterTextBox.Add ("^Connect failed: " + error.ToString() + "^---");
	}
}