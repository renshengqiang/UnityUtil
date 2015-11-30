using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ContextMenuEditor : Editor {
	[MenuItem("Assets/Create/ExtendBehavior")]
	private static void CreateExtendBehavior()
	{
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
		NewExtendBehaviorWindow.InitPath (path);
	}
}

public class NewExtendBehaviorWindow : EditorWindow
{
	private string path;
	
	public static void InitPath(string path)
	{
		NewExtendBehaviorWindow window = (NewExtendBehaviorWindow)EditorWindow.GetWindow (typeof(NewExtendBehaviorWindow));
		window.path = path;
		window.title = "NewBehavior";
		window.minSize = new Vector2 (100, 40);
	}	
	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		string hehaviorName = "NewExtendBehavior";
		hehaviorName = EditorGUILayout.TextField(hehaviorName);
		EditorGUILayout.EndHorizontal();
		if (GUILayout.Button("OK") && !string.IsNullOrEmpty(hehaviorName)) 
		{
			_CreateExtendBehavior(path, hehaviorName);
			Close ();
		}
	}
	private static void _CreateExtendBehavior(string path, string behaviorName)
	{
		string content = System.IO.File.ReadAllText(@Application.dataPath + "/Utils/ExtendBehaviorTemplate.cs");
		Debug.Log (@content);
		content = content.Replace("ExtendBehaviorTemplate", behaviorName);
		File.WriteAllText (path + "/" + behaviorName+ ".cs", content);
		AssetDatabase.Refresh ();
	}
}
