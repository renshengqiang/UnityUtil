using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ContextMenuEditor : Editor {
	[MenuItem("Assets/Create/ExtendBehavior")]
	private static void CreateExtendBehavior()
	{
		_CreateExtendBehavior ("hello");
	}

	private static void _CreateExtendBehavior(string behaviorName)
	{
		string content = System.IO.File.ReadAllText(@Application.dataPath + "/Utils/ExtendBehaviorTemplate.txt");
		string[] guids = Selection.assetGUIDs;
		string path = string.Empty;
		foreach (string guid in guids) {
			path = AssetDatabase.GUIDToAssetPath (guid);
			break;
		}
		if (false == Directory.Exists (path)) 
		{
			path = System.IO.Path.GetDirectoryName (path);
		}
		Debug.Log (content);
		string.Format (content, "behaviorName");
		File.WriteAllText (path + "/" + behaviorName+ ".cs", content);
		AssetDatabase.Refresh ();
	}
}
