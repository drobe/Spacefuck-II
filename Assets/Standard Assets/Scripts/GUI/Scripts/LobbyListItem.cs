using UnityEngine;
using System.Collections;

[System.Serializable]
public class LobbyListItem
{
	public bool valid = false;
	public Rect rect1;
	public Rect rect2;
	public string s;
	public string ss;
	public float age;
	
	public string Get()
	{
		return s + "(" + ss + ")";
	}
}