using UnityEngine;
using System.Collections;

[System.Serializable]
public class CodeAssociation
{
	public string			Name;
	
	public KeyCode 			Key;
	
	public int 				axis;
	public bool				invert;
	public float			deadzone;
	public float			repeat;
	private float			_NextRepeat;
	
	public CommandCode		UpAxis;
	public CommandCode		DownAxis;
	public CommandCode		NeutralAxis;
	
	public CommandCode 		UpCommand;
	public CommandCode		HoldCommand;
	public CommandCode 		DownCommand;
}

public abstract class InputDevice : ScriptableObject 
{
	public InputDeviceType type;
	public bool network 	= false;
	public bool doesExist 	= false;
	
	protected virtual void OnEnable()
	{
		// Register ourselves with the InputManager.
		Spacefuck.InputManager.Register(this);
	}
	protected virtual void OnDestroy()
	{
		Spacefuck.InputManager.UnRegister(this);
	}
	
	public abstract CommandCode[] Update();
}

