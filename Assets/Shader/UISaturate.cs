using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISaturate : MonoBehaviour {
    [SerializeField]
    private Material saturateMat;
    private Material saturateMatMore;
    [SerializeField]
    private Image[] m_extraImages;
    private Image[] m_images;

    private Material[] m_ImageMaterials;
    private Material[] m_ExtraImageMaterials;
    void Awake()
    {
        CheckImg();
        if (null != saturateMat)
        {
            saturateMat = MoreMaterialsManager.Instance.GetMaterial(saturateMat, false);
            saturateMat.SetFloat("_Saturate", 0.0f);//设置为灰色的材质
            saturateMatMore = MoreMaterialsManager.Instance.GetMaterial(saturateMat, true);
            saturateMatMore.SetFloat("_Saturate", 0.0f);//设置为灰色的材质
        }  
    }


    public void SetSaturate(bool saturate,float saturateValue = 0.0f)
    {
        if (saturateMat == null) return;
        CheckImg();

        saturateMat.SetFloat("_Saturate", saturateValue);
        if(saturateMatMore != null)
        {
            saturateMatMore.SetFloat("_Saturate", saturateValue);
        }

        if (m_images != null)
        {
            for (int i = 0; i < m_images.Length; ++i)
            {
                m_images[i].SetAllDirty();                
                if (saturate)
                {
                    if (null != m_images[i].sprite && m_images[i].sprite.name.Contains(".alpha"))//isMoreSprite
                    {
                        m_images[i].material = saturateMatMore;
                    }
                    else
                    {
                        m_images[i].material = saturateMat;
                    }
                }
                else
                {
                    m_images[i].material = m_ImageMaterials[i];
                }
                m_images[i].SetMaterialDirty();
            }
        }

        if (m_extraImages != null)
        {
            for (int i = 0; i < m_extraImages.Length; ++i)
            {
                m_extraImages[i].SetAllDirty();                
                if (saturate)
                {
                    //m_extraImages[i].material = saturateMat;
                    if (null != m_extraImages[i].sprite && m_extraImages[i].sprite.name.Contains(".alpha"))//isMoreSprite
                    {
                        m_extraImages[i].material = saturateMatMore;
                    }
                    else
                    {
                        m_extraImages[i].material = saturateMat;
                    }
                }
                else
                {
                    m_extraImages[i].material = m_ExtraImageMaterials[i];
                }
                m_extraImages[i].SetMaterialDirty();
            }
        }
    }

    private void CheckImg()
    {
        if (m_images == null)
        {
            m_images = GetComponentsInChildren<Image>(true);
        }

        if (null == m_ImageMaterials)
        {
            m_ImageMaterials = new Material[null == m_images ? 0 :m_images.Length];
            for(int i = 0 ; i < m_ImageMaterials.Length ; i++)
            {
                m_ImageMaterials[i] = m_images[i].material;
            }
        }
        if (null == m_ExtraImageMaterials)
        {
            m_ExtraImageMaterials = new Material[null == m_extraImages ?0:m_extraImages.Length];
            for (int i = 0; i < m_ExtraImageMaterials.Length; i++)
            {
                m_ExtraImageMaterials[i] = m_extraImages[i].material;
            }
        }
    }
}