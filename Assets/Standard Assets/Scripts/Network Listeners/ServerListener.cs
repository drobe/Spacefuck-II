using UnityEngine;
using System.Collections;

public class ServerListener : MonoBehaviour 
{
	static private ServerListener instance;
	
	byte[] serverListenerIn = new byte[1024];
	System.Net.Sockets.Socket serverListenerSocket;
	System.Net.IPEndPoint endPoint;
	
	System.Net.Sockets.Socket serverBroadcastSocket;
	System.Net.IPEndPoint endPointBroadcast;
	
	public string Header = "SF][";
	
	public bool isSending		= false;
	public bool isListening		= false;
	
	public float broadcastDelay = 2.0f;
	public float nextBroadcast	= 0.0f;
	
	void Awake()
	{
		instance = this;
	}
	
	// Use this for initialization
	void Start () 
	{
		serverListenerSocket = new System.Net.Sockets.Socket(
			System.Net.Sockets.AddressFamily.InterNetwork,
			System.Net.Sockets.SocketType.Dgram, 
			System.Net.Sockets.ProtocolType.Udp);
		serverListenerSocket.EnableBroadcast = true;
		endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any,9999);
		serverListenerSocket.Bind(endPoint);
		StartListening();
		
		serverBroadcastSocket = new System.Net.Sockets.Socket(
			System.Net.Sockets.AddressFamily.InterNetwork,
			System.Net.Sockets.SocketType.Dgram, 
			System.Net.Sockets.ProtocolType.Udp);
		serverBroadcastSocket.EnableBroadcast = true;
		endPointBroadcast = new System.Net.IPEndPoint(System.Net.IPAddress.Broadcast,9999);
		
//		print (endPointBroadcast.Address.ToString());
	}
			
	static void GetServerBroadcastMessage(System.IAsyncResult result)
	{
		ServerListener sl = (ServerListener) result.AsyncState;
		int numBytes = sl.serverListenerSocket.EndReceive(result);
		
		if (numBytes > 0)
		{
			string message = System.Text.Encoding.Unicode.GetString(sl.serverListenerIn);
			string header = message.Substring(0, instance.Header.Length);
			if (header == instance.Header)
			{
				int x = (int)message[instance.Header.Length];
				int y = (int)message[instance.Header.Length + 1];
				LobbyList.Add(message.Substring(instance.Header.Length + 2),x,y);
			}
		}
		sl.StartListening();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Time.realtimeSinceStartup > nextBroadcast)
		{
			if (Network.isServer)
			{
				char x = (char)(NetworkCommunicator.numPlayers + 1);
				char y = (char)2;
				string message = Header + x + y + Network.player.ipAddress;
				
				byte[] outMessage = System.Text.Encoding.Unicode.GetBytes(message);
				serverBroadcastSocket.SendTo(outMessage, endPointBroadcast);
			}
			nextBroadcast = Time.realtimeSinceStartup + broadcastDelay;
		}
	}
	
	void OnDestroy()
	{
		serverListenerSocket.Close();
		serverBroadcastSocket.Close();
	}
	
	void StartListening()
	{
		serverListenerSocket.BeginReceive(
			serverListenerIn,
			0,
			serverListenerIn.Length,
			System.Net.Sockets.SocketFlags.None,
			GetServerBroadcastMessage,
			this);
	}
}
