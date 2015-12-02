using UnityEngine;
using System.Collections;

public class SharedObjDelegate : MonoBehaviour {
    public string path;
    public bool useLightMap = false;
    public int lightMapID;
    public Vector4 lightmapTilingOffset;

    void Awake()
    {
        Object obj = Resources.Load(path);
        if (obj != null)
        {
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            Transform trans = go.transform;
            trans.parent = transform.parent;
            trans.localPosition = transform.localPosition;
            trans.localRotation = transform.localRotation;
            trans.localScale = transform.localScale;

            if (useLightMap)
            {
                Renderer renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.lightmapIndex = lightMapID;
                    renderer.lightmapTilingOffset = lightmapTilingOffset;
                }
            }
        }
    }
}
