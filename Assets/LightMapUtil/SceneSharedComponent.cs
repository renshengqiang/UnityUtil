using UnityEngine;
using System.Collections;

namespace SceneShareUtil
{
    public enum Quality
    {
        QUALITY_LOW,
        QUALITY_MID,
        QUALITY_HIGH
    }

    /// <summary>
    /// 这个类用于标记场景中的共享物体和品质物体
    /// 加上这个脚本的都是共享物体，如果随品质不同显示则使用品质进行判断
    /// </summary>
    public class SceneSharedComponent : MonoBehaviour
    {
        // can edit in window
        private bool useLightMap = false;
        private bool useQualityProp = false;
        private Quality objQuality;

        // change in build
        private string prefabPath = "";
        private int lightMapId = -1;
        private Vector4 lightMapTilingOffset = Vector4.zero;

        public bool UseLightMap
        {
            get { return useLightMap; }
            set { useLightMap = value; }
        }

        public bool UseQualityProp
        {
            get { return useQualityProp; }
            set { useQualityProp = value; }
        }

        public Quality ObjQuality
        {
            get { return objQuality; }
            set { objQuality = value; }
        }

        public string PrefabPath
        {
            get { return prefabPath; }
            set { prefabPath = value; }
        }

        public int LightMapID
        {
            get { return lightMapId; }
            set { lightMapId = value; }
        }

        public Vector4 LightMapTilingOffset
        {
            get { return lightMapTilingOffset; }
            set { lightMapTilingOffset = value; }
        }

        /// <summary>
        /// 如果是品质物体，并且品质不符合需求的时候就直接返回。否则就去加载。
        /// </summary>
        void Awake()
        {
            // 加上品质判断你的逻辑
            if (!string.IsNullOrEmpty(prefabPath))
            {
                Object obj = Resources.Load(prefabPath);
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
                            renderer.lightmapIndex = lightMapId;
                            renderer.lightmapTilingOffset = lightMapTilingOffset;
                        }
                    }
                }
            }
        }
    }

}
