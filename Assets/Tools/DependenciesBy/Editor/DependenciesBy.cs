using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

[InitializeOnLoad]
public class DependenciesBy : Editor
{    
	private static ObjectRepo repo;
	static DependenciesBy()
	{
		repo = new ObjectRepo ();
		repo.Initialize ();
	}
		
	/// <summary>
	/// 选中当前哪些对象依赖于当前所选中的对象
	/// </summary>
	[MenuItem("Assets/Select Dependencies By")]
	private static void SelectDependenciesBy()
	{
		string[] selections = Selection.assetGUIDs;
		List<string> dependenciesBy = new List<string> ();
		List<UnityEngine.Object> dependenciesByPaths = new List<UnityEngine.Object> ();
		for (int i=0; selections != null && i<selections.Length; ++i) 
		{
			List<string> lst = repo.GetDepencenciesBy(selections[i]);
			if(null != lst)
				dependenciesBy.AddRange(lst);
		}
		for (int i=0; i<dependenciesBy.Count; ++i) 
		{
			Debug.Log(dependenciesBy[i]);
			dependenciesByPaths.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(dependenciesBy[i]), typeof(UnityEngine.Object)));
		}
		Selection.objects = dependenciesByPaths.ToArray ();
        ShowSelectionInProjectHierarchy();
	}

    static void ShowSelectionInProjectHierarchy()
    {
        var pbType = GetType("UnityEditor.ProjectBrowser");
        var meth = pbType.GetMethod("ShowSelectedObjectsInLastInteractedProjectBrowser",
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static);
        meth.Invoke(null, null);
    }

    // helper method to find a tyep of a given name
    static Type GetType(string name)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var asm in assemblies)
        {
            var type = asm.GetType(name);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }
}

/// <summary>
/// Description of every ObjectItem
/// </summary>
public class ObjectItem
{
	public string selfId;
	public List<string> depencenciesBy = new List<string>();
}

public class ObjectRepo
{
	private Dictionary<string, ObjectItem> dicObjecs;
	private static string configFilePath = Application.dataPath + "/../Library/DependenciesBy.config";

	public void Initialize()
	{
		if (FileUtil.FileExist (configFilePath)) 
		{
			Deserialize ();
		}
		else 
		{
			ParseFromRepo();
			Serialize();
		}
	}

	public void Finialize()
	{
		Serialize ();
	}

	public List<string> GetDepencenciesBy(string guid)
	{
		if (dicObjecs.ContainsKey (guid)) {
			return dicObjecs [guid].depencenciesBy;
		} else {
			return null;
		}
	}

	private void ParseFromRepo()
	{
		dicObjecs = new Dictionary<string, ObjectItem>();
		string[] assets = AssetDatabase.GetAllAssetPaths ();
		for (int i = 0; assets != null && i < assets.Length; ++i) {
			string[] dependencies = AssetDatabase.GetDependencies(new string[]{assets[i]});
			string guidDepBy = AssetDatabase.AssetPathToGUID(assets[i]);
			for(int j=0; j<dependencies.Length;++j)
			{
				if(dependencies[j] != assets[i])
				{
					string guid = AssetDatabase.AssetPathToGUID(dependencies[j]);
					ObjectItem item = null;
					dicObjecs.TryGetValue(guid, out item);
					if(null == item)
					{
						item = new ObjectItem();
						item.selfId = guid;
						item.depencenciesBy = new List<string>();
						dicObjecs.Add(guid, item);
					}
					if(false == item.depencenciesBy.Contains(guidDepBy))
					{
						item.depencenciesBy.Add(guidDepBy);
						Debug.Log(assets[i] + "Dep: " + dependencies[j]);
					}
				}
			}
		}
	}

	private void Deserialize()
	{
		dicObjecs = new Dictionary<string, ObjectItem>();
		string[] allLines = FileUtil.ReadAllLines (configFilePath);
		for (int index = 0; allLines != null && index < allLines.Length; ++index) {
			string[] objectAndDep = allLines[index].Split(new char[]{':'}, System.StringSplitOptions.RemoveEmptyEntries);
			if(objectAndDep.Length < 2)
			{
				Debug.LogError("Config file is broken, suggests rebuild it when needed or free");
			}
			else
			{
				ObjectItem item = new ObjectItem();
				item.selfId = objectAndDep[0];
				item.depencenciesBy = new List<string>();
				string[] dependencies = objectAndDep[1].Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries);
				if(dependencies != null)
				{
					item.depencenciesBy.AddRange(dependencies);
				}
				dicObjecs.Add(item.selfId, item);
			}
		}
	}

	private void Serialize()
	{
		string content = "";

		if (null == dicObjecs) 
		{
			Debug.LogError("Need to Initialze First");
			return;
		}
		foreach (KeyValuePair<string, ObjectItem> keyValue in dicObjecs)
		{
			ObjectItem item = keyValue.Value;
			content += item.selfId + ":";
			for(int index = 0; item.depencenciesBy != null && index < item.depencenciesBy.Count; ++index)
			{
				content += item.depencenciesBy[index] + ";";
			}
			content += "\r\n";
		}

		FileUtil.SaveToFile (content, configFilePath);
	}
}