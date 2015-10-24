using System;
using UnityEngine;

public static class Extension
{
	public static void SetPositionX (this Transform trans, float x)
	{
		Vector3 toTrans = trans.localPosition;
		toTrans.x = x;
		trans.localPosition = toTrans;
	}

	public static void SetPositionY (this Transform trans, float y)
	{
		Vector3 toTrans = trans.localPosition;
		toTrans.y = y;
		trans.localPosition = toTrans;
	}

	public static void SetPositionZ (this Transform trans, float z)
	{
		Vector3 toTrans = trans.localPosition;
		toTrans.z = z;
		trans.localPosition = toTrans;
	}
}