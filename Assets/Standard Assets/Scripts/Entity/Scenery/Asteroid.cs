using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Asteroid : SFEntity
{
	private const float MIN_SIZE 				= 1;
	private const float MAX_SIZE 				= 5;
	private const float MIN_MASS				= 100;
	private const float MAX_MASS				= 200;
	
	static private Queue<Asteroid> 	unassigned 	= new Queue<Asteroid>(200);
	static private List<Asteroid> 	assigned 	= new List<Asteroid>(200);
	
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
		assigned.Remove(this);
		unassigned.Enqueue(this);
		ObjectPool.Destroy(this.gameObject);
	}
	
//	void Explode()
//	{
//		int size = Mathf.CeilToInt(transform.localScale.magnitude * 0.5f);
//		for(int i = 0; i < size * 4; i++)
//		{
//			Vector2 offset2D = Random.insideUnitCircle * size;
//			Vector3 offset3D = new Vector3(offset2D.x,offset2D.y,0);
//			GameplayController.MakeBullet(transform.position + offset3D,offset3D * Random.Range(5,25));
//		}
//		Destroy(gameObject);
//	}
	
	public static GameObject Instantiate(Vector3 position, Quaternion rotation)
	{
		if (unassigned.Count == 0 && assigned.Count == 0)
		{
			// No
			Debug.LogWarning("No Asteroids have been pooled.");
			return null;
		}
		if (unassigned.Count > 0)
		{
			Asteroid p 				= unassigned.Dequeue();
			
			p.transform.position 	= position;
			p.transform.rotation 	= rotation;
			
			p.transform.localScale 	= Vector3.one * Random.Range(MIN_SIZE, MAX_SIZE);
			p.rigidbody.mass 		= Random.Range(MIN_MASS, MAX_MASS);
			
			p.gameObject.SetActive(true);
			
			return p.gameObject;
		}
		
		// No
		Debug.LogWarning("All asteroids have been assigned.");
		return null;
	}
	
	override protected void Explode()
	{
		int numBullets = Mathf.FloorToInt(rigidbody.mass * 0.05f);
		
		Bounds b = collider.bounds;
		Vector3 position3D = b.center;
		
		for(int i = 0; i < numBullets; i++)
		{
			Vector3 bulletPosition = position3D;
			bulletPosition.x = Random.Range(b.min.x,b.max.x);
			bulletPosition.y = Random.Range(b.min.y,b.max.y);
			
			GameObject go = Bullet.Instantiate(bulletPosition,Quaternion.identity);
			Vector3 velocity = (bulletPosition - b.center).normalized;
			go.rigidbody.velocity = velocity * Random.Range(shrapnelMinSpeed,shrapnelMaxSpeed);
		}
		
		ObjectPool.Destroy(this.gameObject);
	}
}
