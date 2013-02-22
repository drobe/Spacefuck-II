using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : SFEntity
{
	static private Queue<Bullet> unassigned 	= new Queue<Bullet>(200);
	static private List<Bullet> assigned 		= new List<Bullet>(200);
	
	void Awake()
	{
		unassigned.Enqueue(this);
	}
	
	void Start()
	{
	}
	
	void OnDestroy()
	{
	}
	
	void OnEnable()
	{
		if (firstEnable)
		{
			firstEnable = false;
		}
		else
		{
			assigned.Add(this);
		}
	}
	
	void OnDisable()
	{
		if (assigned.Remove(this))
		{
			unassigned.Enqueue(this);
		}
	}
	
	public static GameObject Instantiate(Vector3 position, Quaternion rotation)
	{
		if (unassigned.Count == 0 && assigned.Count == 0)
		{
			// No
			Debug.LogWarning("No Bullets have been pooled.");
			return null;
		}
		//print (unassigned.Count.ToString() + " | " + assigned.Count.ToString());
		if (unassigned.Count > 0)
		{
			Bullet p = unassigned.Dequeue();
			
			p.transform.position = position;
			p.transform.rotation = rotation;
			
			p.gameObject.SetActive(true);
			
			return p.gameObject;
		}
		else
		{
			Bullet p = assigned[0];
			
			p.transform.position = position;
			p.transform.rotation = rotation;
			
			assigned.RemoveAt(0);
			assigned.Add(p);
			
			return p.gameObject;
		}
	}
	
	override protected void Explode()
	{
	}
}
