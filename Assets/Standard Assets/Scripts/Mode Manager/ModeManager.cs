using UnityEngine;
using System.Collections;

public class ModeManager : MonoBehaviour 
{
	private static GameMode mode 	= GameMode.MainMenu;
	private static bool setMode		= false;
	
	public GUISkin					skin;
	public GameObject menu;
	public LocalHostingScreen 		localHostingScreen;
	public NetworkJoinScreen 		networkJoinScreen;
	public NetworkHostingScreen 	networkHostingScreen;
	public DrawnBoundary			drawnBoundary;
	// Use this for initialization
	void Start () 
	{
		Application.runInBackground = true;
		menu.SetActive(true);
		
		localHostingScreen.enabled 		= false;
		networkJoinScreen.enabled 		= false;
		networkHostingScreen.enabled 	= false;
		drawnBoundary.enabled			= false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (setMode)
		{
			GoToMode ();
			setMode = false;
		}
		TypewriterTextBox.External_Update();
	}
	
	void OnGUI()
	{
//		Rect rect = new Rect(0,0,100,20);
//		GameMode[] modes = (GameMode[])System.Enum.GetValues(typeof(GameMode));
//		
//		foreach (GameMode m in modes)
//		{
//			if (GUI.Button(rect,m.ToString()))
//			{
//				GoTo (m);
//			}
//			rect.y += rect.height;
//		}
		GUI.skin = skin;
		if (mode != GameMode.MainMenu)
		{
			if (GUI.Button(new Rect(2,2,20,20),"<"))
			{
				if (Network.isServer)
				{
					NetworkCommunicator.SendMessage(NetworkMessage.Quit,0);
				}
				GoTo (GameMode.MainMenu);
			}
		}
	}
	
	static public void GoTo(GameMode targetMode)
	{
		setMode 	= true;
		mode 		= targetMode;
	}
	
	private void GoToMode()
	{
		switch(mode)
		{
		case GameMode.Boot:
			Camera.mainCamera.cullingMask = 1;
			Debug.LogWarning("Never goto to boot.");
			break;
		case GameMode.MainMenu:
			EnableMenu();
			break;
		case GameMode.HostLocal:
			EnableLocalHost();
			break;
		case GameMode.HostNetwork:
			EnableNetHost();
			break;
		case GameMode.JoinNetwork:
			EnableNetJoin();
			break;
		case GameMode.Gameplay:
			EnableGameplay();
			break;
		default:
			Debug.LogWarning("Unrecognized or obsolete Game Mode: " + mode.ToString());
			EnableMenu();
			break;
		}
	}
	
	private void EnableMenu()
	{
		menu.SetActive(true);
		
		localHostingScreen.enabled 		= false;
		networkJoinScreen.enabled 		= false;
		networkHostingScreen.enabled 	= false;
		drawnBoundary.enabled			= false;
		
		Camera.mainCamera.cullingMask = 1;
		Time.timeScale = 0;
		ObjectPool.Clear();
		Network.Disconnect(10);
		NetworkHostingScreen.autoServe 	= false;
		
		Spacefuck.InputManager.Clean();
	}
	private void EnableNetJoin()
	{
		menu.SetActive(false);
		
		localHostingScreen.enabled 		= false;
		networkJoinScreen.enabled 		= true;
		networkHostingScreen.enabled 	= false;
		drawnBoundary.enabled			= false;
		NetworkJoinScreen.amIready		= false;
		
		Camera.mainCamera.cullingMask = 1;
		Time.timeScale = 0;
		ObjectPool.Clear();
	}
	private void EnableNetHost()
	{
		menu.SetActive(false);
		
		localHostingScreen.enabled 		= false;
		networkJoinScreen.enabled 		= false;
		networkHostingScreen.enabled 	= true;
		drawnBoundary.enabled			= false;
		
		NetworkHostingScreen.areTheyReady = false;
		Camera.mainCamera.cullingMask = 1;
		Time.timeScale = 0;
		if (!Network.isServer)
		{
			Network.InitializeServer(1,6666,true);
		}
		ObjectPool.Clear();
	}
	private void EnableLocalHost()
	{
		menu.SetActive(false);
		
		localHostingScreen.enabled 		= true;
		networkJoinScreen.enabled 		= false;
		networkHostingScreen.enabled 	= false;
		drawnBoundary.enabled			= false;
		
		Camera.mainCamera.cullingMask = 1;
		Time.timeScale = 0;
		ObjectPool.Clear();
	}
	private void EnableGameplay()
	{
		menu.SetActive(false);
		
		localHostingScreen.enabled 		= false;
		networkJoinScreen.enabled 		= false;
		networkHostingScreen.enabled 	= false;
		drawnBoundary.enabled			= true;
		
		Camera.mainCamera.cullingMask = -1;
		Time.timeScale = 1;
	}
}
