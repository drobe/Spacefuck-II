using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planetoid : SFEntity
{
	private const float MIN_SIZE 				= 10;
	private const float MAX_SIZE 				= 20;
	private const float MIN_MASS				= 1000;
	private const float MAX_MASS				= 2000;
	
	public float asteroidEjectionForceMin		= 5;
	public float asteroidEjectionForceMax		= 5;
	
	public float asteroidGenerationModifier		= 0.01f;
	
	static private Queue<Planetoid> unassigned 	= new Queue<Planetoid>(200);
	static private List<Planetoid> assigned 	= new List<Planetoid>(200);
	
	
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
//		int numAsteroids 	= Mathf.CeilToInt(size * 0.5f);
//		int numBullets		= size * 4;
//		
//		for(int i = 0; i < numAsteroids; i++)
//		{
//			Vector2 offset2D = Random.insideUnitCircle;
//			Vector3 offset3D = new Vector3(offset2D.x,offset2D.y,0);
//			GameplayController.MakeAsteroid(transform.position + offset3D, Vector3.zero);
//		}
//		for(int i = 0; i < numBullets; i++)
//		{
//			Vector2 offset2D = Random.insideUnitCircle * size;
//			Vector3 offset3D = new Vector3(offset2D.x,offset2D.y,0);
//			GameplayController.MakeBullet(transform.position + offset3D, offset3D);
//		}
//		Destroy(gameObject);
//	}
	
	public static GameObject Instantiate(Vector3 position, Quaternion rotation)
	{
		if (unassigned.Count == 0 && assigned.Count == 0)
		{
			// No
			Debug.LogWarning("No Planetoids have been pooled.");
			return null;
		}
		if (unassigned.Count > 0)
		{
			Planetoid p 			= unassigned.Dequeue();
			
			p.transform.position 	= position;
			p.transform.rotation 	= rotation;
			
			p.transform.localScale 	= Vector3.one * Random.Range(MIN_SIZE, MAX_SIZE);
			p.rigidbody.mass 		= Random.Range(MIN_MASS, MAX_MASS);
			
			p.gameObject.SetActive(true);
			
			return p.gameObject;
		}
		
		// No
		Debug.LogWarning("All planetoids have been assigned.");
		return null;
	}
	
	override protected void Explode()
	{
		int numAsteroids = Mathf.FloorToInt(rigidbody.mass * asteroidGenerationModifier);
		int numBullets = Mathf.Min(20, Mathf.FloorToInt(rigidbody.mass - numAsteroids));
		
		Bounds b = collider.bounds;
		Vector3 position3D = b.center;
		Vector3 toCenter = (-b.center).normalized;
		position3D += (toCenter * b.size.magnitude * 0.5f);
		
		for(int i = 0; i < numAsteroids; i++)
		{
			Vector3 bulletPosition = position3D;
			bulletPosition.x = Random.Range(b.min.x,b.max.x);
			bulletPosition.y = Random.Range(b.min.y,b.max.y);
			
			GameObject go = Asteroid.Instantiate(bulletPosition,Quaternion.identity);
			Vector3 velocity = (bulletPosition - b.center).normalized;
			go.rigidbody.velocity = velocity * Random.Range(asteroidEjectionForceMin,asteroidEjectionForceMax);
		}
		
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
