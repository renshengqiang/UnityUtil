using UnityEngine;
using System.Collections;

public class TestLoadingSceneUseAB : MonoBehaviour {

	void Awake()
    {
        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {
        WWW www = new WWW("file:///" + Application.dataPath + "/Bundle/lightmap.assetbundle");
        yield return www;

        AssetBundle ab = www.assetBundle;
        Application.LoadLevel("lightmap");
    }
}
