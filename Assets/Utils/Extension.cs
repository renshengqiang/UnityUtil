using System;
using UnityEngine;
using System.Diagnostics;

public static class Extension
{
	public static void Log (this MonoBehaviour mono, object obj)
	{
		string stackInfo = new StackTrace().ToString();
		UnityEngine.Debug.Log (obj + stackInfo);
	}
}