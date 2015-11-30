using UnityEngine;
using System.Collections;

public delegate void  ResourceCallBack(string path, Object obj);
public class ResourceManager : MonoBehaviour
{
    public static bool useBundle = false;

    #region Singleton
    public static ResourceManager pInstance
    {
        get
        {
            if (mInstance == null)
                mInstance = (ResourceManager)Object.FindObjectOfType(typeof(ResourceManager));

            if (mInstance == null)
            {
                GameObject GO = new GameObject("ResourceManager", typeof(ResourceManager));
                GO.hideFlags = GO.hideFlags | HideFlags.HideAndDontSave;	// Only hide it if this manager was autocreated
                mInstance = GO.GetComponent<ResourceManager>();
            }

            DontDestroyOnLoad(mInstance.gameObject);
            return mInstance;
        }
    }
    static ResourceManager mInstance;
    #endregion

    public void LoadAsync(string path, ResourceCallBack callback)
    {
        if (useBundle)
        {
            BundleLoader.pInstance.LoadBundle("file:///" + Application.dataPath + "/Bundle/" + path + ".assetbundle", (AssetBundle ab) =>
            {
                if (ab != null && ab.mainAsset != null)
                {
                    Object obj = ab.mainAsset;
                    callback(path, obj);
                }
                else
                {
                    callback(path, null);
                }
            });
        }
        else
        {
            Object obj = Resources.Load(path);
            callback(path, obj);
        }
    }
}
