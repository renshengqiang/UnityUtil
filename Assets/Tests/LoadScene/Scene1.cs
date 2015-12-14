using UnityEngine;
using System.Collections;
using Log;

public class Scene1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadScene());
	}

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.1f);
        Application.LoadLevelAsync("Scene2");
        Logger.Log("LoadLevel2 async");
    }

    //// Update is called once per frame
    //void Update () {
    //    Logger.Log("Scene1: Update");
    //}

    void FixedUpdate()
    {
        Logger.Log("Scene1: FixedUpdate");
    }

    void OnDestroy()
    {
        Logger.Log("Scene1: Destory");
    }

    public void OnLevelWasLoaded(int level)
    {
        Logger.Log("In Scene1: " + level + " was loaded");
    }
}
