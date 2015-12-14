using UnityEngine;
using System.Collections;
using Log;

public class Scene2 : MonoBehaviour {

    void Awake()
    {
        Logger.Log("Scene2 Awake");
    }

	// Use this for initialization
	void Start () {
        Logger.Log("Scene2 Started");
        Logger.Log("current level: " + Application.loadedLevelName);
	}

	// Update is called once per frame
	void Update () {
	 
	}

    void OnDestroy()
    {
        Logger.Log("Scene2: Destory");
    }

    public void OnLevelWasLoaded(int level)
    {
        Logger.Log("In Scene2: " + level + " was loaded");
    }
}
