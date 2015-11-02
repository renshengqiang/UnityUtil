using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class DependenciesBy : AssetPostprocessor
{    
	private static ObjectRepo repo;
	private static bool initialized = false;
	 
	static DependenciesBy()
	{
		Initialize ();
	}

	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
	                                           string[] movedAssets, string[] movedFromAssetPaths)
	{
		Initialize ();
		for (int i=0; importedAssets != null && i < importedAssets.Length; ++i) 
		{
			repo.ImportAsset(importedAssets[i]);
		}
		for (int i=0; deletedAssets != null && i < deletedAssets.Length; ++i) 
		{
			repo.DeleteAsset(deletedAssets[i]);
		}
	}
		
	private static void Initialize()
	{
		if (false == initialized)
		{
			repo = new ObjectRepo ();
			repo.Initialize ();
			initialized = true;
		}
	}
	/// <summary>
	/// Select Objects which depend by selected Objects
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
			dependenciesByPaths.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(dependenciesBy[i]), typeof(UnityEngine.Object)));
		}
		Selection.objects = dependenciesByPaths.ToArray ();
        ShowSelectionInProjectHierarchy();
	}

    private static void ShowSelectionInProjectHierarchy()
    {
		Type pbType = GetType("UnityEditor.ProjectBrowser");
		MethodInfo meth = pbType.GetMethod("ShowSelectedObjectsInLastInteractedProjectBrowser",
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static);
        meth.Invoke(null, null);
    }

    // helper method to find a tyep of a given name
    private static Type GetType(string name)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i=0; assemblies != null && i < assemblies.Length; ++i)
		{
			Type type = assemblies[i].GetType(name);
			if (type != null)
			{
				return type;
			}
		}
        return null;
    }
}

public class ObjectRepo
{
	private Dictionary<string, List<string>> dicDepBy;
	private Dictionary<string, List<string>> dicDep;
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
		if (dicDepBy.ContainsKey (guid)) {
			return dicDepBy [guid];
		} else {
			return null;
		}
	}

	public void ImportAsset(string assetPath)
	{
		string guid = AssetDatabase.AssetPathToGUID (assetPath);
		string[] dependencies = AssetDatabase.GetDependencies(new string[]{assetPath});
		List<string> lstDep = null;

		if (false == dicDep.TryGetValue(assetPath, out lstDep)) 
		{
			lstDep = new List<string>();
			dicDep[assetPath] = lstDep;
		}
		for (int i=0; dependencies != null && i < dependencies.Length; ++i) 
		{
			if(dependencies[i] == assetPath) continue;	// Ignore myself

			// add dicDepBy
			string depGuid = AssetDatabase.AssetPathToGUID(dependencies[i]);
			List<string> lstDepBy = null;
			dicDepBy.TryGetValue(depGuid, out lstDepBy);
			if(null == lstDepBy)
			{
				lstDepBy = new List<string>();
				dicDepBy[depGuid] = lstDepBy;
			}
			if(false == lstDepBy.Contains(guid))
			{
				lstDepBy.Add(guid);
				Debug.Log(assetPath + "Dep: " + dependencies[i]);
			}

			// add dicDep
			if(false == lstDep.Contains(depGuid))
			{
				lstDep.Add(depGuid);
			}
		}

		Serialize ();
		Debug.Log ("Added: " + assetPath);
	}

	public void ParseFromRepo()
	{
		dicDepBy = new Dictionary<string, List<string>>();
		dicDep = new Dictionary<string, List<string>> ();
		string[] assets = AssetDatabase.GetAllAssetPaths ();
		for (int i = 0; assets != null && i < assets.Length; ++i) 
		{
			ImportAsset(assets[i]);
		}
	}

	public void DeleteAsset(string assetPath)
	{
		string guid = AssetDatabase.AssetPathToGUID (assetPath);
		List<string> lstDep = null;
		List<string> lstDepBy = null;

		dicDep.TryGetValue (guid, out lstDep);
		dicDepBy.TryGetValue (guid, out lstDepBy);

		for (int i=0; lstDep != null && i<lstDep.Count; ++i) 
		{
			List<string> innerLstDepBy = null;
			dicDepBy.TryGetValue(lstDep[i], out innerLstDepBy);
			if(null == innerLstDepBy)
			{
				Debug.LogWarning("Assets Depencencies is broken, suggest repairt it " + AssetDatabase.GUIDToAssetPath(lstDep[i]));
			}
			else
			{
				if(innerLstDepBy.Contains(guid))
				{
					innerLstDepBy.Remove(guid);
				}
				else
				{
					Debug.LogWarning("Assets Depencencies is broken, suggest repairt it " + AssetDatabase.GUIDToAssetPath(guid));
				}
			}

			Serialize ();
		}

		for (int i=0; lstDepBy != null && i<lstDepBy.Count; ++i) 
		{
			List<string> innerLstDep = null;
			dicDep.TryGetValue(lstDepBy[i], out innerLstDep);
			if(null == innerLstDep)
			{
				Debug.LogWarning("Assets Depencencies is broken, suggest repairt it " + AssetDatabase.GUIDToAssetPath(lstDepBy[i]));
			}
			else
			{
				if(innerLstDep.Contains(guid))
				{
					innerLstDep.Remove(guid);
				}
				else
				{
					Debug.LogWarning("Assets Depencencies is broken, suggest repairt it " + AssetDatabase.GUIDToAssetPath(guid));
				}
			}
		}

		Debug.Log("Deleted：" + assetPath);
	}

	private void Deserialize()
	{
		dicDepBy = new Dictionary<string, List<string>>();
		dicDep = new Dictionary<string, List<string>> ();
		string[] allLines = File.ReadAllLines (configFilePath);
		for (int index = 0; allLines != null && index < allLines.Length; ++index) {
			string[] objectAndDep = allLines[index].Split(new char[]{':'}, System.StringSplitOptions.RemoveEmptyEntries);
			if(objectAndDep.Length < 2)
			{
				Debug.LogError("Config file is broken, suggests rebuild it when needed or free");
			}
			else
			{
				List<string> lstDepBy = new List<string>();
				string[] dependencies = objectAndDep[1].Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries);
				if(dependencies != null)
				{
					lstDepBy.AddRange(dependencies);
				}
				dicDepBy.Add(objectAndDep[0], lstDepBy);

				for(int i=0; dependencies != null && i<dependencies.Length; ++i)
				{
					List<string> lstDep = null;
					dicDep.TryGetValue(dependencies[i], out lstDep);
					if(null == lstDep) 
					{
						lstDep = new List<string>();
						dicDep.Add(dependencies[i], lstDep);
					}
					lstDep.Add(objectAndDep[0]);
				}
			}
		}
	}

	private void Serialize()
	{
		string content = "";

		if (null == dicDepBy) 
		{
			Debug.LogError("Need to Initialze First");
			return;
		}
		foreach (KeyValuePair<string, List<string>> keyValue in dicDepBy)
		{
			List<string> lstDepBy = keyValue.Value;
			content += keyValue.Key + ":";
			for(int index = 0; lstDepBy != null && index < lstDepBy.Count; ++index)
			{
				content += lstDepBy[index] + ";";
			}
			content += "\r\n";
		}

		using (FileStream fileStream = new FileStream(configFilePath, 
		                                   File.Exists(configFilePath) ? FileMode.Truncate : FileMode.OpenOrCreate))
		{
			byte[] bytes = System.Text.Encoding.Default.GetBytes(content);
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Close();
			fileStream.Dispose();
		}
	}
}