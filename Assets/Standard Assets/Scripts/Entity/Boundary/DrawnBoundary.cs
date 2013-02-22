using UnityEngine;
using System.Collections;

public class DrawnBoundary : Boundary 
{
	public GUISkin skin;
	public Rect boxRect = new Rect();
	public int depth = 0;
	
	void Update()
	{
		Vector3[] positions = new Vector3[2];
		Vector3 invertedMin = collider.bounds.min;
		invertedMin.y *= -1;
		invertedMin.z = 0;
		Vector3 invertedMax = collider.bounds.max;
		invertedMax.y *= -1;
		invertedMax.z = 0;
		positions[0] = Camera.mainCamera.WorldToScreenPoint(invertedMin);
		positions[1] = Camera.mainCamera.WorldToScreenPoint(invertedMax);
		
//		boxRect = new Rect(positions[0].x, positions[0].y,
//							positions[1].x - positions[0].x, 
//							positions[1].y - positions[0].y);
		boxRect = new Rect(0,0,0,0);
		boxRect.x = positions[0].x;
		boxRect.y = Screen.height - positions[0].y;
		boxRect.width = positions[1].x - positions[0].x;
		boxRect.height =  positions[0].y - positions[1].y;
		//boxRect.center = Camera.mainCamera.WorldToScreenPoint(collider.bounds.center);

	}
	
	void OnGUI()
	{
		GUI.skin = skin;
		GUI.depth = depth;
		GUI.Box(boxRect,"");
	}
}
