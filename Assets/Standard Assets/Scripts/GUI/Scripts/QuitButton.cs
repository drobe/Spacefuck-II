using UnityEngine;
using System.Collections;

public class QuitButton : MonoBehaviour 
{
	public GUISkin skin;
	void OnGUI()
	{
		GUI.skin = skin;
		
		Rect quit		= new Rect(Screen.width - 22, 0, 20, 20);
		Rect window		= new Rect(Screen.width - 44, 0, 20, 20);
		if (Screen.fullScreen)
		{
			if (GUI.Button(quit,"X"))
			{
				Application.Quit();
			}
			if (GUI.Button(window,"="))
			{
				Screen.fullScreen = !Screen.fullScreen;
			}
		}
	}
}
