using UnityEngine;
using System.Collections;
using Log;

public class BundleDemo : MonoBehaviour {

    void Awake()
    {
        ResourceManager.pInstance.LoadAsync("enemy", LoadCallback);
    }

    void LoadCallback(string path, Object obj)
    {
        if (obj != null)
        {
            GameObject.Instantiate(obj);
        }
        else
        {
            Logger.LogError("Load " + path + " error");
        }
    }
}
