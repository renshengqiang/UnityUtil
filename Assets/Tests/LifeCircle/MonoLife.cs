using UnityEngine;
using System.Collections;
using Log;

public class MonoLife : MonoBehaviour {

	void Awake()
	{
		Logger.Log ("MonoLife Awake");
	}
	
	void OnEnable()
	{
		Logger.Log ("MonoLife OnEnable");
	}

	// Use this for initialization
	void Start () {
		Logger.Log ("MonoLife Start");
	}

	void FixedUpdate()
	{

	}

	public void CustomFunction()
	{
		Logger.Log ("MonoLife CustomFunction");
	}


	// Update is called once per frame
	void Update () {
	}

	void LateUpdate()
	{

	}

	void OnDisable()
	{

	}

	void OnDestroy()
	{

	}
}
