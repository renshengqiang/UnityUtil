using UnityEngine;
using System.Collections;
using UnityEditor;

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
}
