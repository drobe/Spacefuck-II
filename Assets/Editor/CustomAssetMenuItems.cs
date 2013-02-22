using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;			// Path

public static class CustomAssetMenuItems 
{
	[MenuItem("Assets/Create/InputDevice/Controller")]
	public static void CreateControllerDevice ()
	{
		ScriptableObjectUtility.CreateAsset<ControllerDevice> ();
	}
	[MenuItem("Assets/Create/InputDevice/Network")]
	public static void CreateNetworkDevice ()
	{
		ScriptableObjectUtility.CreateAsset<NetworkDevice> ();
	}
	[MenuItem("Assets/Create/InputDevice/Keyboard")]
	public static void CreateKeyboardDevice ()
	{
		ScriptableObjectUtility.CreateAsset<KeyboardDevice> ();
	}
	[MenuItem("Assets/Create/InputDevice/AI")]
	public static void CreateAIDevice ()
	{
		ScriptableObjectUtility.CreateAsset<AIDevice> ();
	}
	
	#region Make Text File
	[MenuItem ("Assets/Create/Text File")]
	public static void MakeTXTFile ()
	{
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New Text.txt");
 
		File.Open(assetPathAndName,FileMode.CreateNew).Close();
		
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
	}
	#endregion
}
