using UnityEngine;
using System.Collections;
using Log;

public delegate void BundleLoaderCallback(AssetBundle bundle);
public class BundleLoader : MonoBehaviour
{
    #region Singleton
    public static BundleLoader pInstance
    {
        get
        {
            if (mInstance == null)
                mInstance = (BundleLoader)Object.FindObjectOfType(typeof(BundleLoader));

            if (mInstance == null)
            {
                GameObject GO = new GameObject("BundleLoader", typeof(BundleLoader));
                GO.hideFlags = GO.hideFlags | HideFlags.HideAndDontSave;	// Only hide it if this manager was autocreated
                mInstance = GO.GetComponent<BundleLoader>();
            }

            DontDestroyOnLoad(mInstance.gameObject);
            return mInstance;
        }
    }
    static BundleLoader mInstance;
    #endregion

    public void LoadBundle(string pathname, BundleLoaderCallback callback)
    {
        WWWLoadWorker worker = new WWWLoadWorker(pathname);
        worker.StartLoad((WWW www) =>
        {
            if (null != www)
            {
                AssetBundle ab = www.assetBundle;
                if (ab != null)
                {
                    callback(ab);
                }
                else
                {
                    Logger.LogError("Load assetbundle " + pathname + " error");
                }
            }
            else
            {
                Logger.LogError("Load " + pathname + " error");
            }
        });
    }
}
