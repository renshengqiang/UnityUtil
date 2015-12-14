using UnityEngine;
using System.Collections;

public class DisableLightMap : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.lightmapIndex = -1;
        }
	}
}
