using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is an input manager that aggregates *virtual* devices and maps them to Input Managed objects.
// This input manager is meant to play friendly with both local and remote (networked) devices, treating both as "essentially the same."
namespace Spacefuck
{
	static public class InputManager 
	{
		public struct PlayerSlot
		{
			public InputDevice 	device;
			public InputManaged managedObject;
		}
		
		// Built-in definitions.
		private const int DEVICE_PRECACHE 					= 10;
		private const int COMMAND_CODE_PRECACHE				= 100;
		private const int NUMBER_OF_PLAYERS					= 4;
		
		
		static private PlayerSlot[] PlayerSlots 			= new PlayerSlot[NUMBER_OF_PLAYERS];
		static private List<CommandCode> CommandCodeQueue 	= new List<CommandCode>(COMMAND_CODE_PRECACHE);
	
				
		// Input Devices add themselves to the input manager when they fall into scope.
		static private List<InputDevice> InputDevices 		= new List<InputDevice>(DEVICE_PRECACHE);
		
		// A default Input Managed object. Set at runtime.
		// The default Input Managed object receives all CommandCodes returned from device updates.
		static private InputManaged defaultManagedObject 	= null;
		
		// Call update every frame to get control events from registered Input Devices.
		static public void Update()
		{
			for(int i = 0; i < PlayerSlots.Length; i++)
			{
				PlayerSlot slot = PlayerSlots[i];
				CommandCode[] commands;
				
				if (slot.managedObject && slot.device)
				{
					commands = slot.device.Update();
					slot.managedObject.Send(commands);
					
					if (Network.isClient && slot.device.network)
					{
						NetworkCommunicator.SendCommand(i,commands);
					}
				}
			}
			if (defaultManagedObject != null)
			{
				foreach(InputDevice device in InputDevices)
				{
					CommandCodeQueue.AddRange(device.Update());
				}

				defaultManagedObject.Send(CommandCodeQueue.ToArray());
				CommandCodeQueue.Clear();
			}
			
		}
		
		// Register an Input Device with the Input Manager
		static public void Register(InputDevice device)
		{
			if (device == null)
			{
				Debug.LogWarning("Don't register a null device.");
				return;
			}
			InputDevices.Add(device);
		}
		
		// Unregister a Input Device from the Input Manager.
		// This only stops an Input Device from being updated.
		// ID associations will still persist, and they will need to be cleared if you ever intend to unload a device.
		static public void UnRegister(InputDevice device)
		{
			InputDevices.Remove(device);
		}
		
		// Returns an array of Input Devices.
		// This is used for associating Input Devices to Input Managed objects.
		static public InputDevice[] GetDevices()
		{
			List<InputDevice> activeDevices = new List<InputDevice>(InputDevices.Count);
			foreach (InputDevice d in InputDevices)
			{
				if (d.doesExist)
				{
					activeDevices.Add(d);
				}
			}
			return activeDevices.ToArray();
		}
		
		// Returns an array of Device to Input Managed associations.
		static public PlayerSlot[] GetPlayerSlots()
		{
			return PlayerSlots;
		}
		
		
//		// Performs a soft-clear on the input manager.
//		// Resets all Player Slots.
//		static public void SoftClear()
//		{
//			PlayerSlots = new PlayerSlot[NUMBER_OF_PLAYERS];
//		}
//		
//		
//		// Performs a hard-clear on the input manager.
//		// Clears everything; devices, managed objects, and player slots.
//		static public void DeepClear()
//		{
//			PlayerSlots = new PlayerSlot[NUMBER_OF_PLAYERS];
//			InputDevices.Clear();
//		}
		
		// Sets the default Input Managed object.
		// Use this for your main menu or default listener.
		// The DefaultInputManaged object recieves all CommandCodes.
		static public void SetDefault(InputManaged managedObject)
		{
			defaultManagedObject = managedObject;
		}
		
		// Assign an Input Managed object to a Player Slot.
		static public void SetPlayer(int i, InputManaged im)
		{
			if (i < PlayerSlots.Length && i >= 0)
			{
				PlayerSlots[i].managedObject = im;
			}
		}
		
		// Assign an Input Device to a Player Slot
		static public void SetDevice(int i, InputDevice d)
		{
			if (i < PlayerSlots.Length && i >= 0)
			{
				PlayerSlots[i].device = d;
			}
		}
		
		// Send an array of CommandCodes directly to a Player Slot.
		static public void DirectSend(int i, CommandCode[] codes)
		{
			if (i < PlayerSlots.Length && i >= 0)
			{
				if (PlayerSlots[i].managedObject)
				{
					PlayerSlots[i].managedObject.Send(codes);
				}
			}
		}
		
		static public void Clean()
		{
			for (int i = 0; i < PlayerSlots.Length; i++)
			{
				SetDevice(i,null);
			}
		}
		static public void NetPrep()
		{
			for (int i = 0; i < PlayerSlots.Length; i++)
			{
				SetDevice(i,NetworkDevice.device);
			}
		}
	}
}
