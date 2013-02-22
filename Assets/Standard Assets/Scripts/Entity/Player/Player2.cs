using UnityEngine;
using System.Collections;

public class Player2 : InputManaged
{
	public float thrustForce;
	public float rotation;
	public float shootForce;
	public float shootCooldown;
	private float _Cooldown;
	public Transform gun;
	
	public bool rotateLeft;
	public bool rotateRight;
	public bool thrust;
	public bool shoot;
	
	void Start()
	{
	}
	
	void OnEnable()
	{
	}
	
	void OnDisable()
	{
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (rotateLeft)
		{
			rigidbody.angularVelocity += Vector3.forward * rotation * Time.fixedDeltaTime;
		}
		if (rotateRight)
		{
			rigidbody.angularVelocity += Vector3.back * rotation * Time.fixedDeltaTime;
		}
		if (thrust)
		{
			rigidbody.velocity += transform.right * thrustForce * Time.fixedDeltaTime;
		}
		if (shoot && !Network.isClient)
		{
			if (_Cooldown < 0)
			{
				_Cooldown 				= shootCooldown;
				GameObject go 			= Bullet.Instantiate(gun.position,Quaternion.identity);
				go.rigidbody.velocity 	= (transform.right * shootForce);
			}
		}
		_Cooldown -= Time.fixedDeltaTime;
		Debug.DrawLine(gun.position, gun.position + (transform.right * 1000), Color.red,Time.fixedDeltaTime);
	}
	
//	void Explode()
//	{
//		for(int i = 0; i < 30; i++)
//		{
//			Vector2 offset2D = Random.onUnitSphere;
//			Vector3 offset3D = new Vector3(offset2D.x,offset2D.y,0);
//			GameplayController.MakeBullet(transform.position + offset3D,offset3D * Random.Range(5,25));
//		}
//		Destroy(gameObject);
//	}
	
	public override void Send (CommandCode[] codes)
	{
		foreach (CommandCode c in codes)
		{
			switch(c)
			{
			case CommandCode.RotateLeft:
				rotateLeft = true;
				break;
			case CommandCode.RotateRight:
				rotateRight = true;
				break;
			case CommandCode.StopRotate:
				rotateLeft 	= false;
				rotateRight = false;
				break;
			case CommandCode.Thrust:
				thrust = true;
				break;
			case CommandCode.StopThrust:
				thrust = false;
				break;
			case CommandCode.Shoot:
				shoot = true;
				break;
			case CommandCode.StopShoot:
				shoot = false;
				break;
			default:
				break;
			}
		}
	}
}
