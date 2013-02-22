using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerDevice : InputDevice  
{
	[System.Serializable]
	protected class KeyCommandAssociation
	{
		public string			Name;
		
		public KeyCode 			Key;
		public float			firstRepeat;
		public float			repeat;
		[System.NonSerialized]
		public float			_NextRepeat;
		
		public CommandCode 		UpCommand;
		public CommandCode		HoldCommand;
		public CommandCode 		DownCommand;
	}
	
	[System.Serializable]
	protected class AxisCommandAssociation
	{
		public enum 			State
		{
			Up,
			Down,
			Neutral
		}
		public string			Name;
		
		public string 			Axis;
		public bool				repeatNeutral;
		public bool				repeatPolar;
		public float			repeat;
		public float			deadZone;
		[System.NonSerialized]
		public float			_NextRepeat;
		
		public CommandCode 		UpCommand;
		public CommandCode		NeutralCommand;
		public CommandCode 		DownCommand;
		
		[System.NonSerialized]
		public State			state;
	}
	
	[SerializeField]
	private KeyCommandAssociation[] KeyCommandAssociations 	= new KeyCommandAssociation[0];
	[SerializeField]
	private AxisCommandAssociation[] AxisCommandAssociations= new AxisCommandAssociation[0];
	private List<CommandCode> CommandCodes;
	[SerializeField]
	private float shutoffCooldown 	= 200f;
	private float _shutoffCooldown	= 0f;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		CommandCodes = new List<CommandCode>(KeyCommandAssociations.Length + AxisCommandAssociations.Length);
	}
	
	public override CommandCode[] Update()
	{
		CommandCodes.Clear();
		float now = Time.realtimeSinceStartup;
				
		foreach (KeyCommandAssociation command in KeyCommandAssociations)
		{
			if (command.UpCommand != CommandCode.None && Input.GetKeyUp(command.Key))
			{
				CommandCodes.Add (command.UpCommand);
			}
			else if (command.DownCommand != CommandCode.None && Input.GetKeyDown(command.Key))
			{
				CommandCodes.Add (command.DownCommand);
				command._NextRepeat = now + command.firstRepeat;
			}
			else if (command.HoldCommand != CommandCode.None && now > command._NextRepeat && Input.GetKey(command.Key))
			{
				CommandCodes.Add(command.HoldCommand);
				command._NextRepeat = now + command.repeat;
			}
		}
		
		foreach (AxisCommandAssociation command in AxisCommandAssociations)
		{
			float axis 		= Input.GetAxisRaw(command.Axis);
			bool sign 		= Mathf.Sign(axis) >= 0;
			float absAxis 	= Mathf.Abs(axis);
			
			//MonoBehaviour.print (name + ": " + command.Name + ": " + axis);
			
			if (absAxis > command.deadZone)
			{
				if (sign)
				{
					if (command.state != AxisCommandAssociation.State.Up)
					{
						CommandCodes.Add(command.UpCommand);
						command.state = AxisCommandAssociation.State.Up;
						command._NextRepeat = now + command.repeat;
					}
					else if (command.repeatPolar && now > command._NextRepeat)
					{
						CommandCodes.Add(command.UpCommand);
						command._NextRepeat = now + command.repeat;
					}
				}
				else
				{
					if (command.state != AxisCommandAssociation.State.Down)
					{
						CommandCodes.Add(command.DownCommand);
						command.state = AxisCommandAssociation.State.Down;
						command._NextRepeat = now + command.repeat;
					}
					else if (command.repeatPolar && now > command._NextRepeat)
					{
						CommandCodes.Add(command.DownCommand);
						command._NextRepeat = now + command.repeat;
					}
				}
			}
			else
			{
				if (command.state != AxisCommandAssociation.State.Neutral)
				{
					CommandCodes.Add(command.NeutralCommand);
					command.state = AxisCommandAssociation.State.Neutral;
					command._NextRepeat = now + command.repeat;
				}
				else if (command.repeatNeutral && now > command._NextRepeat)
				{
					CommandCodes.Add(command.NeutralCommand);
					command._NextRepeat = now + command.repeat;
				}
			}
		}
		
		if (now < 1f)
		{
			doesExist = false;
			return new CommandCode[0];
		}
		
		if (CommandCodes.Count > 0)
		{
			doesExist = true;
			_shutoffCooldown = now + shutoffCooldown;
		}
		else if (now > _shutoffCooldown)
		{
			doesExist = false;
		}
		return CommandCodes.ToArray();
	}
}
