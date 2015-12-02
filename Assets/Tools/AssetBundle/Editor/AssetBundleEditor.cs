using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Util;

public class AssetBundleEditor : Editor {
	/// <summary>
    /// 将所选择的的物体和物体有依赖关系的对象一起打包
    /// </summary>
    [MenuItem("Assets/Build AssetBundle From Selection - Track dependencies")]
    public static void BuildAssetBundle()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "assetbundle");

        if (path.Length != 0)
        {
            // Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            AssetBundleTool.BuildAssetWithDependencies(selection, path);
        }
    }

    /// <summary>
    /// 将所选择的的场景进行打包
    /// </summary>
    [MenuItem("Assets/Build Scenes From Selection")]
    public static void BuildScenes()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "assetbundle");

        if (path.Length != 0)
        {
            // Build the resource file from the active selection.
            string[] selection = Selection.assetGUIDs;

            if (selection != null)
            {
                List<string> lstScenes = new List<string>();
                for (int i = 0; i < selection.Length; ++i)
                {
                    string pathName = AssetDatabase.GUIDToAssetPath(selection[i]);
                    if (pathName.EndsWith(".unity"))
                    {
                        lstScenes.Add(pathName);
                    }
                }
                AssetBundleTool.BuildScenesWithDependencies(lstScenes.ToArray(), path);
            }
        }
    }
}
