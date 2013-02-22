using UnityEngine;
using UnityEditor;
using System.Collections;

public class InputManagerEditor : EditorWindow  
{
	void OnGUI()
	{
		// Devices
		Spacefuck.InputManager.PlayerSlot[] slots = Spacefuck.InputManager.GetPlayerSlots();
		
		int count = 0;
		foreach (Spacefuck.InputManager.PlayerSlot slot in slots)
		{
			EditorGUILayout.LabelField("Slot: " + count.ToString());

			EditorGUILayout.ObjectField("Device",slot.device,typeof(InputDevice),false);
			EditorGUILayout.ObjectField("Object",slot.managedObject,typeof(InputManaged),false);
			count ++;
		}

	}
	
	[MenuItem("Editor/Input Manager Viewer")]
    public static void Init()
    {
        // Get the window, show it, and hand it focus
        InputManagerEditor window = EditorWindow.GetWindow<InputManagerEditor>("InputManager Viewer");
        window.Show();
        window.Focus();
		window.autoRepaintOnSceneChange = true;
    }
}
