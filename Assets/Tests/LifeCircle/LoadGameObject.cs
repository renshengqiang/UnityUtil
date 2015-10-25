using UnityEngine;
using System.Collections;
using Util;
using Log;

public class LoadGameObject : MonoBehaviour {

	void Awake()
	{
		Logger.Log ("LoadGameObject Awake");
		GameObject go = PrefabUtil.LoadInstantiate ("MonoLife");
		if(null != go)
		{
			Logger.Log("Set MonoLift active = true");
			go.SetActive(true);
		}

		MonoLife mono = go.GetComponent<MonoLife> ();
		if (null != mono) {
			mono.CustomFunction();
		}
	}
}
