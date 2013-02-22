using UnityEngine;
using System.Collections;

// The network device is a blank device.
// Networking, oddly enough, is handled through other means.
// This device is just a marker for which player slots should be marked for networking.
public class NetworkDevice : InputDevice  
{
	public static NetworkDevice device;
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		device 		= this;
		doesExist 	= true;
	}
	
	public override CommandCode[] Update()
	{
		return new CommandCode[0];
	}
}
