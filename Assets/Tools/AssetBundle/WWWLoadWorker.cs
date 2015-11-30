using UnityEngine;
using System.Collections;


public delegate void WWWLoadCallback(WWW www);

public class WWWLoadWorker 
{
    private string pathname;
    private WWW www = null;
    public WWW wwwIns
    {
        get { return www; }
    }

    public WWWLoadWorker(string pathname)
    {
        this.pathname = pathname; 
    }

    public void StartLoad(WWWLoadCallback callback)
    {
        BundleLoader.pInstance.StartCoroutine(LoadCoroutine(callback));
    }

    private IEnumerator LoadCoroutine(WWWLoadCallback callback)
    {
        www = new WWW(pathname);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            callback(null);
        }
        else
        {
            callback(www);
        }
    }
}
