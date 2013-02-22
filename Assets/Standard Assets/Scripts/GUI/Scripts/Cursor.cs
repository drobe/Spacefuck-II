using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour 
{
	public Texture2D cursorTexture;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector2 position = Input.mousePosition;
		if ((position.x < 0 || position.x > Screen.width ||
			position.y < 0 || position.y > Screen.height ) 
			&& Screen.showCursor)
		{
			Screen.showCursor = true;
		}
		else if (Screen.showCursor)
		{
			Screen.showCursor = false;
		}
	}
	
	void OnGUI()
	{
		Vector2 position = Input.mousePosition;
		if (!Screen.showCursor)
		{
			GUI.DrawTexture(new Rect(position.x, Screen.height - position.y, cursorTexture.width, cursorTexture.height),cursorTexture);
		}
	}
}
