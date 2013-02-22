using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyboardDevice : InputDevice 
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
	
	[SerializeField]
	private KeyCommandAssociation[] KeyCommandAssociations 	= new KeyCommandAssociation[0];
	private List<CommandCode> CommandCodes;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		CommandCodes = new List<CommandCode>(KeyCommandAssociations.Length);
		doesExist = true;
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
		return CommandCodes.ToArray();	
	}
}
