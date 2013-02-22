using UnityEngine;
using UnityEditor;
using System.Collections;

public class CommonEditor : Editor
{
	[DrawGizmo(GizmoType.NotSelected)]
	static void DrawBoundaries(Boundary b, GizmoType gizmoType)
	{   
		if (b.collider)
		{
			Vector3 bounds = b.collider.bounds.size;
			if (b.GetType() == typeof(InnerArenaController))
			{
				Gizmos.color = Color.green;
			}
			else if (b.GetType() == typeof(OuterArenaController))
			{
				Gizmos.color = Color.red;
			}
			Gizmos.DrawWireCube(b.collider.bounds.center,bounds);
		}
	}
	[DrawGizmo(GizmoType.SelectedOrChild | GizmoType.Active)]
	static void DrawFilledBoundaries(Boundary b, GizmoType gizmoType)
	{   
		if (b.collider)
		{
			Vector3 bounds = b.collider.bounds.size;
			Color green = Color.green;
			Color red = Color.red;
			Color white = Color.white;
			green.a = red.a = white.a = 0.5f;
			if (b.GetType() == typeof(InnerArenaController))
			{
				Gizmos.color = green;
			}
			else if (b.GetType() == typeof(OuterArenaController))
			{
				Gizmos.color = red;
			}
			else
			{
				Gizmos.color = white;
			}
			Gizmos.DrawCube(b.collider.bounds.center,bounds);
		}
	}
	
	//	[DrawGizmo(GizmoType.SelectedOrChild | GizmoType.NotSelected)]
//	static void DrawBounds(GameObject g, GizmoType gizmoType)
//	{   
//		if (g.collider)
//		{
//			Collider b = g.collider;
//			Gizmos.color = Color.green;
//			Gizmos.DrawWireSphere(b.bounds.min, 5);
//			Gizmos.color = Color.blue;
//			Gizmos.DrawWireSphere(b.bounds.max, 5);
//			Gizmos.color = Color.grey;
//			Gizmos.DrawWireCube(b.bounds.center,b.bounds.size);
//		}
//	}
	
	[DrawGizmo(GizmoType.SelectedOrChild | GizmoType.Active)]
	static void DrawFilledBoundaries(CustomGUI b, GizmoType gizmoType)
	{   
		
	}
}
