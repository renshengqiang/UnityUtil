using UnityEngine;
using System.Collections;

public class XXXComponent : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EventDispatcher.dispatcher.RequestForSomething(Handler);
        Debug.Log("Before Destroy " + gameObject);
        Destroy(gameObject);
	}

    void Handler()
    {
        Debug.Log("After Destroy " + gameObject);
    }
}
