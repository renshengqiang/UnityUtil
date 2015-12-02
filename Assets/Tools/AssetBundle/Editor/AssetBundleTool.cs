using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Util;

public class AssetBundleTool 
{
    public static void BuildAssetWithDependencies(Object[] lstAssets, string outPathName, BuildTarget target = BuildTarget.WebPlayer)
    {
        BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.CollectDependencies |
                                                          BuildAssetBundleOptions.CompleteAssets |
                                                          BuildAssetBundleOptions.DeterministicAssetBundle;
        if (lstAssets == null || lstAssets.Length < 1)
        {
            Debug.LogError("AssetBundleTool::BuildAssetWithDependencies " + lstAssets + " is null");
        }
        else
        {
            if (lstAssets.Length == 1)
            {
                BuildPipeline.BuildAssetBundle(lstAssets[0], null, outPathName, buildAssetBundleOptions);
            }
            else
            {
                BuildPipeline.BuildAssetBundle(null, lstAssets, outPathName, buildAssetBundleOptions);
            }
        }
    }

    public static void BuildScenesWithDependencies(string[] lstScenes, string outPathName, BuildTarget target = BuildTarget.WebPlayer)
    {
        foreach (EditorBuildSettingsScene oneScene in EditorBuildSettings.scenes)
        {
            EditorApplication.OpenScene(oneScene.path);
            SceneSharedObj[] sharedObjs = GameObject.FindObjectsOfType<SceneSharedObj>();
            for (int i = 0; sharedObjs != null && i < sharedObjs.Length && sharedObjs[i] != null; ++i)
            {
                Transform trans = sharedObjs[i].gameObject.transform;
                Object prefab = PrefabUtility.GetPrefabParent(sharedObjs[i].gameObject) as Object;
                string path = AssetDatabase.GetAssetPath(prefab);

                GameObject go = new GameObject("SharedObj", typeof(SharedObjDelegate));
                SharedObjDelegate com = go.GetComponent<SharedObjDelegate>();
                go.transform.parent = trans.parent;
                go.transform.position = trans.position;
                go.transform.localRotation = trans.localRotation;
                go.transform.localScale = trans.localScale;
                if (com != null)
                {
                    com.path = StringUtil.GetFileNameRelativeToResources(path);
                    Renderer renderer = sharedObjs[i].GetComponent<Renderer>();
                    if (sharedObjs[i].useLightMap && renderer != null)
                    {
                        com.useLightMap = true;
                        com.lightMapID = renderer.lightmapIndex;
                        com.lightmapTilingOffset = renderer.lightmapTilingOffset;
                    }
                }
                GameObject.DestroyImmediate(sharedObjs[i].gameObject);
            }
            EditorApplication.SaveScene();
        }

        if (lstScenes == null || lstScenes.Length < 1)
        {
            Debug.LogError("AssetBundleTool::BuildScenesWithDependencies " + lstScenes + " is null");
        }
        else
        {
            for (int i = 0; i < lstScenes.Length; ++i)
            {
                GameObject scene = AssetDatabase.LoadAssetAtPath(lstScenes[i], typeof(GameObject)) as GameObject;
                if(scene != null)
                {
                    SceneSharedObj[] sharedObjs = scene.GetComponentsInChildren<SceneSharedObj>();
                    for (int j = 0; sharedObjs != null && j < sharedObjs.Length; ++j)
                    {

                    }
                }
            }

            BuildPipeline.BuildStreamedSceneAssetBundle(lstScenes, outPathName, target);
        }
    }
}
