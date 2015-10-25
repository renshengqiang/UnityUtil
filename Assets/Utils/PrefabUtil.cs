using UnityEngine;
using System.Collections;
using Log;

namespace Util
{
	public class PrefabUtil
	{
		public static GameObject LoadInstantiate (string path)
		{
			GameObject ret = null;
			GameObject go = Resources.Load (path) as GameObject;
			if (null == go) {
				Logger.Log ("gameobject at " + path + "not exist");
			} else {
				ret = GameObject.Instantiate (go) as GameObject;
			}
			return ret;
		}
	}
}