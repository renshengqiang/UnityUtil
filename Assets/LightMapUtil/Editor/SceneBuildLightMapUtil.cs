using UnityEngine;
using UnityEditor;
using System.Collections;
using Util;
using SceneShareUtil;

[CustomEditor(typeof(SceneSharedComponent))]
public class SceneSharedComponentEditor : Editor
{
    int choice = 0;
    public override void OnInspectorGUI()
    {
        SceneSharedComponent sharedCom = target as SceneSharedComponent;
        bool useLightMap = EditorGUILayout.Toggle("useLightMap", sharedCom.UseLightMap);
        sharedCom.UseLightMap = useLightMap;
        bool useQualityProp = EditorGUILayout.Toggle("useQualityProp", sharedCom.UseQualityProp);
        sharedCom.UseQualityProp = useQualityProp;
        if (useQualityProp)
        {
            string[] quality = new string[]{"low", "mid", "high"};
            choice = EditorGUILayout.Popup("Quality", choice, quality);
            if (choice == 0)
            {
                sharedCom.ObjQuality = Quality.QUALITY_LOW;
            }
            else if (choice == 1)
            {
                sharedCom.ObjQuality = Quality.QUALITY_MID;
            }
            else
            {
                sharedCom.ObjQuality = Quality.QUALITY_HIGH;
            }
        }
        //base.OnInspectorGUI();
    }
}

public class SceneBuildLightMapUtil
{
	// Use this for initialization
	public static void CleanSharedAndQualityObjsInScene () 
    {
        foreach (EditorBuildSettingsScene oneScene in EditorBuildSettings.scenes)
        {
            EditorApplication.OpenScene(oneScene.path);
            SceneSharedComponent[] sharedObjs = GameObject.FindObjectsOfType<SceneSharedComponent>();
            for (int i = 0; sharedObjs != null && i < sharedObjs.Length && sharedObjs[i] != null; ++i)
            {
                Transform trans = sharedObjs[i].gameObject.transform;
                Object prefab = PrefabUtility.GetPrefabParent(sharedObjs[i].gameObject) as Object;
                string path = AssetDatabase.GetAssetPath(prefab);

                GameObject go = new GameObject("SharedObj", typeof(SceneSharedComponent));
                SceneSharedComponent com = go.GetComponent<SceneSharedComponent>();
                go.transform.parent = trans.parent;
                go.transform.position = trans.position;
                go.transform.localRotation = trans.localRotation;
                go.transform.localScale = trans.localScale;
                if (com != null)
                {
                    com.UseLightMap = sharedObjs[i].UseLightMap;
                    com.UseQualityProp = sharedObjs[i].UseQualityProp;
                    com.ObjQuality = sharedObjs[i].ObjQuality;
                    com.PrefabPath = StringUtil.GetFileNameRelativeToResources(path);
                    Renderer renderer = sharedObjs[i].GetComponent<Renderer>();
                    if (sharedObjs[i].UseLightMap && renderer != null)
                    {
                        com.LightMapID = renderer.lightmapIndex;
                        com.LightMapTilingOffset = renderer.lightmapTilingOffset;
                    }
                    else
                    {
                        com.UseLightMap = false;
                    }
                }
                GameObject.DestroyImmediate(sharedObjs[i].gameObject);
            }
            EditorApplication.SaveScene();
        }
	}
}
