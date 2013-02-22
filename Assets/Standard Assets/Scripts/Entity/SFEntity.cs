using UnityEngine;
using System.Collections;

public class SFEntity : MonoBehaviour
{
//	public bool DEBUG_EXPLODE = false;
//	public bool DEBUG_DESTROY = false;
	
	protected bool firstEnable = true;
	public float shrapnelMinSpeed = 5;
	public float shrapnelMaxSpeed = 5;
	
//	protected void Update()
//	{
//		if (DEBUG_EXPLODE)
//		{
//			DEBUG_EXPLODE = false;
//			gameObject.SendMessage("Explode");
//		}
//		if (DEBUG_DESTROY)
//		{
//			DEBUG_EXPLODE = false;
//			gameObject.SendMessage("DestroySF");
//		}
//	}
	
//	protected void OnMouseUpAsButton()
//	{
//		DEBUG_EXPLODE = true;
//	}
	
	virtual protected void Explode()
	{
		int numBullets = Mathf.FloorToInt(rigidbody.mass);
		
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
		GameplayController2.PlayerHasDied();
		ObjectPool.Destroy(this.gameObject);
	}
	virtual protected void DestroySF()
	{
		ObjectPool.Destroy(this.gameObject);
	}
}

