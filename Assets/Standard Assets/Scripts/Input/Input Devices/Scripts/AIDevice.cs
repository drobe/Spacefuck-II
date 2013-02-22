using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIDevice : InputDevice  
{
	public bool DEBUG_CAN_PANIC 	= false;
	public bool DEBUG_CAN_RECOVER 	= false;
	public bool DEBUG_CAN_ATTACK 	= false;
	public bool DEBUG_CAN_PRINT		= false;
	
	public static AIDevice device;
	static private Spacefuck.InputManager.PlayerSlot[] slots = new Spacefuck.InputManager.PlayerSlot[0];
	static private int frameCount = -1;
	static private bool active;
	private List<CommandCode> CommandCodes;
	
	// If our distance to the center is greater than panic distance, we will immediately turn to center and thrust non-stop.
	public float panicDistance;
	public float panicTurnCuttoff;
	public float panicThrustAngle;
	
	public float recoverySpeed;
	public float recoveryTurnCuttoff;
	public float recoveryThrustAngle;
	
	public float attackTurnCuttoff;
	public float attackAngle;
	
	public CommandCode[] codes	= new CommandCode[4];
	// Keeping it simple.
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		device 		= this;
		doesExist 	= true;
		
		CommandCodes = new List<CommandCode>(10);
	}
	
	public override CommandCode[] Update()
	{
		if (frameCount == Time.frameCount || !active)
		{
			return new CommandCode[0];
		}
		
		frameCount = Time.frameCount;
		
		int playerID = 0;
		foreach (Spacefuck.InputManager.PlayerSlot s in slots)
		{
			CommandCodes.Clear();
			if (!s.device || s.device.type  != InputDeviceType.AI)
			{
				playerID++;
				continue;
			}
			
			
			CommandCodes.Add(codes[2]);
			CommandCodes.Add(codes[4]);
			CommandCodes.Add(codes[6]);
			
			InputManaged o		= 	s.managedObject;
			Vector3 position	= 	o.transform.position;
			
			if (Vector3.Distance(position, Vector3.zero) >= panicDistance && DEBUG_CAN_PANIC)
			{	
				PANIC (o);
			}
			else if (o.rigidbody.velocity.magnitude >= recoverySpeed && DEBUG_CAN_RECOVER)
			{
				STEADY (o);
			}
			else if (DEBUG_CAN_ATTACK)
			{
				ATTACK (o);
			}
			if(DEBUG_CAN_PRINT)
			{
				Debug.Log (frameCount.ToString() +":"+ playerID.ToString() + ">> " + CommandCodes.Count.ToString() + " codes.");
			}
			Spacefuck.InputManager.DirectSend(playerID,CommandCodes.ToArray());
			playerID++;
		}
		
		return new CommandCode[0];
	}
	
	static public void Begin()
	{
		slots = Spacefuck.InputManager.GetPlayerSlots();
		frameCount = 0;
		active = true;
	}
	
	static public void End()
	{
		active = false;
		
		int playerID = 0;
#pragma warning disable 0219
		foreach (Spacefuck.InputManager.PlayerSlot s in slots)
		{	
			Spacefuck.InputManager.DirectSend(playerID,new CommandCode[]{CommandCode.StopRotate, CommandCode.StopShoot, CommandCode.StopThrust});
			playerID++;
		}
#pragma warning restore 0219
	}
	
	private void PANIC(InputManaged o)
	{
		// DON"T TOUCH
		Vector3 position	= 	o.transform.position;
		Vector3 target		=  	-position;
		Vector3 forward		=	o.transform.right;
		Vector3 right		= 	o.transform.up;
		
		float angle 	= 	Vector3.Angle(target, forward);
		float sign 		= 	(Vector3.Dot(position, right) > 0.0f) ? 1.0f: -1.0f;//Mathf.Sign(Vector3.Dot(toCenter, right));
		
		if (Mathf.Abs(angle) > panicTurnCuttoff)
		{
			if (sign > 0.0f)
			{
				CommandCodes.Add(codes[0]);
			}
			else
			{
				CommandCodes.Add(codes[1]);
			}
		}
		
		if (Mathf.Abs(angle) < panicThrustAngle)
		{
			CommandCodes.Add(codes[3]);
			// Thrust
		}
		Debug.DrawLine(position, position + (target * 10),Color.blue,Time.deltaTime);
	}
	
	private void STEADY(InputManaged o)
	{
		Vector3 position	= 	o.transform.position;
		Vector3 target		= 	-o.rigidbody.velocity;
		Vector3 forward		=  	o.transform.right;
		Vector3 right		= 	o.transform.up;
		
		float angle 	= 	Vector3.Angle(target, forward);
		float sign 		= 	(Vector3.Dot(-target, right) > 0.0f) ? 1.0f: -1.0f;//Mathf.Sign(Vector3.Dot(toCenter, right));
		
		if (Mathf.Abs(angle) > recoveryTurnCuttoff)
		{
			if (sign > 0.0f)
			{
				CommandCodes.Add(codes[0]);
			}
			else
			{
				CommandCodes.Add(codes[1]);
			}
		}
		
		if (Mathf.Abs(angle) < recoveryThrustAngle)
		{
			CommandCodes.Add(codes[3]);
			// Thrust
		}
		Debug.DrawLine(position, position + (target * 10),Color.green,Time.deltaTime);
	}
	
	private void ATTACK(InputManaged o)
	{
		Vector3 position	= 	o.transform.position;
		Vector3 target		=	Vector3.zero;
		Vector3 forward		=	o.transform.right;
		Vector3 right		= 	o.transform.up;
		
		float shortestDistance 	= float.PositiveInfinity;
		foreach(Spacefuck.InputManager.PlayerSlot s in slots)
		{
			if (s.managedObject != o && s.managedObject.gameObject.activeSelf)
			{
				float distance = Vector3.Distance(position, s.managedObject.transform.position);
				if (distance < shortestDistance)
				{
					shortestDistance = distance;
					float ETA	= distance * 0.04f;
					target 	= ((s.managedObject.transform.position + (s.managedObject.rigidbody.velocity * ETA)) 
							- (position));
				}
			}
		}
		
		float angle 	= 	Vector3.Angle(target, forward);
		float sign 		= 	(Vector3.Dot(-target, right) > 0.0f) ? 1.0f: -1.0f;//Mathf.Sign(Vector3.Dot(toCenter, right));

		
		if (Mathf.Abs(angle) > attackTurnCuttoff)
		{
			if (sign > 0.0f)
			{
				CommandCodes.Add(codes[0]);
			}
			else
			{
				CommandCodes.Add(codes[1]);
			}
		}
		
		if (Mathf.Abs(angle) < attackAngle)
		{
			CommandCodes.Add(codes[5]);
			// Thrust
		}
		Debug.DrawLine(position, position + (target * 10),Color.white,Time.deltaTime);
	}
}
