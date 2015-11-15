using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Sprites;
using System.Collections.Generic;

namespace MoreFun.UI
{
	/// <summary>
	/// no padding 
	/// left:RGB  right:Alpha
	/// </summary>
	[AddComponentMenu ("UI/MoreImage", 11)]
	public class MoreImage : Image 
	{
        public const float APPENDPIXELSIZE = 8f;
        private float m_RealAppend = APPENDPIXELSIZE;
        private float m_AdjustOuterUVScale;//原本的大小站现在大小的比例
        private float m_AppendPixelScale;//添加的像素站原本大小的比例
		protected static Material m_alphaMaterial;
        private Sprite m_preSprite = null;
        private bool m_NeedInit = true;
        private Vector4 m_PreUV0;
        private Vector4 m_PreUV1;
	    private Type m_PreType = Type.Simple;

        public bool enableLog = false;

        //protected static Material alphaMaterial
        //{
        //    get
        //    {
        //        if (m_alphaMaterial == null)
        //        {
        //            m_alphaMaterial = new Material(Shader.Find("MoreFun/UI-DefaultAlpha"));
        //        }
        //        return m_alphaMaterial;
        //    }
        //}

		//alpha texture dir
		private enum Dir
		{
			NONE = 0,
			LEFT_RIGHT = 1,
			UP_DOWN = 2,
		}

		private Dir m_dir = Dir.NONE;
        /// <summary>
        /// 只检测dir，不计算与切换材质
        /// </summary>
        /// <param name="onCheckDir"></param>
		private void CheckDir(bool onCheckDir = false)
		{
            if (overrideSprite == null)
            {
                m_dir = Dir.NONE;
            }
            else
            {
                m_RealAppend = APPENDPIXELSIZE * pixelsPerUnit;
                if (overrideSprite.name.EndsWith(".lr"))
                {
                    m_dir = Dir.LEFT_RIGHT;
                    if (!onCheckDir)
                    {
                        m_AdjustOuterUVScale = ((overrideSprite.rect.width - m_RealAppend) * 0.5f) / overrideSprite.rect.width;
                        m_AppendPixelScale = m_RealAppend / ((overrideSprite.rect.width - m_RealAppend) * 0.5f);
                    }
                }
                else if (overrideSprite.name.EndsWith(".ud"))
                {
                    m_dir = Dir.UP_DOWN;
                    if (!onCheckDir)
                    {
                        m_AdjustOuterUVScale = ((overrideSprite.rect.height - m_RealAppend) * 0.5f) / overrideSprite.rect.height;
                        m_AppendPixelScale = m_RealAppend / ((overrideSprite.rect.height - m_RealAppend) * 0.5f);
                    }
                }
                else
                {
                    m_dir = Dir.NONE;
                    m_AdjustOuterUVScale = 0.5f;
                }
            }
            if (!onCheckDir)
            {
                m_alphaMaterial = MoreMaterialsManager.Instance.GetMaterial(m_Material, m_dir != Dir.NONE);
                m_Material = m_alphaMaterial;
            }
		}

        public override void SetNativeSize()
        {
            if (overrideSprite != null)
            {
                float w, h;
                w = overrideSprite.rect.width / pixelsPerUnit;
                h = overrideSprite.rect.height / pixelsPerUnit;
                if (m_dir == Dir.LEFT_RIGHT)
                {
                    w = (overrideSprite.rect.width - m_RealAppend) / pixelsPerUnit;
                    w *= 0.5f;
                }
                else if (m_dir == Dir.UP_DOWN)
                {
                    h = (overrideSprite.rect.height - m_RealAppend) / pixelsPerUnit;
                    h *= 0.5f;
                }
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
                SetAllDirty();
            }
        }

        /// <summary>
        /// Returns the material used by this Graphic.
        /// </summary>
        public override Material material
        {
            get
            {
                return base.material;
            }
            set
            {
                if (m_Material == value)
                    return;
                CheckDir(true);
                m_alphaMaterial = MoreMaterialsManager.Instance.GetMaterial(value, m_dir != Dir.NONE);
                m_Material = m_alphaMaterial;
                SetMaterialDirty();
            }
        }
		protected override void OnFillVBO (List<UIVertex> vbo)
        {
            if (m_preSprite != base.sprite || m_preSprite != base.overrideSprite)//button事件是直接改变了image的overrideSprite
            {
               // Debug.LogError(m_preSprite + " , " + base.sprite +  "  ,  " + base.overrideSprite );
                m_NeedInit = true;
                m_preSprite = overrideSprite;
            }
            ////check dir and check overrideSprite is null
            if (m_NeedInit)
            {
                CheckDir();
            }

			if(m_dir == Dir.NONE)
			{
				base.OnFillVBO(vbo);
                return;
			}
		    if (m_PreType != type)
		    {
		        m_NeedInit = true;
		        m_PreType = type;
		    }
			switch(type)
			{
				case Type.Simple:
					GenerateSimpleSprite(vbo, preserveAspect);
					break;
				case Type.Sliced:
					GenerateSlicedSprite(vbo);
					break;
				case Type.Tiled:
					GenerateTiledSprite(vbo);
					break;
				case Type.Filled:
					GenerateFilledSprite(vbo,preserveAspect);
					break;
            }
            m_NeedInit = false;
        }

		Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
		{
			var padding = overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
			var size = Vector2.zero;
			if(overrideSprite != null)
			{
				if(m_dir == Dir.LEFT_RIGHT)
                    size = new Vector2((overrideSprite.rect.width - m_RealAppend) * 0.5f, overrideSprite.rect.height);
				else if(m_dir == Dir.UP_DOWN)
                    size = new Vector2(overrideSprite.rect.width, (overrideSprite.rect.height - m_RealAppend) * 0.5f);
				else
					size = new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);
			}
			
			Rect r = GetPixelAdjustedRect();
			// Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));
			
			int spriteW = Mathf.RoundToInt(size.x);
			int spriteH = Mathf.RoundToInt(size.y);
			
			var v = new Vector4(
				padding.x / spriteW,
				padding.y / spriteH,
				(spriteW - padding.z) / spriteW,
				(spriteH - padding.w) / spriteH);
			
			if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
			{
				var spriteRatio = size.x / size.y;
				var rectRatio = r.width / r.height;
				
				if (spriteRatio > rectRatio)
				{
					var oldHeight = r.height;
					r.height = r.width * (1.0f / spriteRatio);
					r.y += (oldHeight - r.height) * rectTransform.pivot.y;
				}
				else
				{
					var oldWidth = r.width;
					r.width = r.height * spriteRatio;
					r.x += (oldWidth - r.width) * rectTransform.pivot.x;
				}
			}
			
			v = new Vector4(
				r.x + r.width * v.x,
				r.y + r.height * v.y,
				r.x + r.width * v.z,
				r.y + r.height * v.w
				);
			
			return v;
		}

		Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
		{
			for (int axis = 0; axis <= 1; axis++)
			{
				float combinedBorders = border[axis] + border[axis + 2];
				if (rect.size[axis] < combinedBorders && combinedBorders != 0)
				{
					float borderScaleRatio = rect.size[axis] / combinedBorders;
					border[axis] *= borderScaleRatio;
					border[axis + 2] *= borderScaleRatio;
				}
			}
			return border;
		}

		void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax, Vector2 uv1Min, Vector2 uv1Max)
		{
			v.position = new Vector3(posMin.x, posMin.y, 0);
			v.uv0 = new Vector2(uvMin.x, uvMin.y);
			v.uv1 = new Vector2(uv1Min.x, uv1Min.y);
			vbo.Add(v);
			
			v.position = new Vector3(posMin.x, posMax.y, 0);
			v.uv0 = new Vector2(uvMin.x, uvMax.y);
			v.uv1 = new Vector2(uv1Min.x, uv1Max.y);
			vbo.Add(v);
			
			v.position = new Vector3(posMax.x, posMax.y, 0);
			v.uv0 = new Vector2(uvMax.x, uvMax.y);
			v.uv1 = new Vector2(uv1Max.x, uv1Max.y);
			vbo.Add(v);
			
			v.position = new Vector3(posMax.x, posMin.y, 0);
			v.uv0 = new Vector2(uvMax.x, uvMin.y);
			v.uv1 = new Vector2(uv1Max.x, uv1Min.y);
			vbo.Add(v);
		}

		Vector4 GetAdjustOuterUV(Vector4 uv)
		{
			if(m_dir == Dir.LEFT_RIGHT)
                uv.z = uv.x + (uv.z - uv.x) * m_AdjustOuterUVScale;
			else if(m_dir == Dir.UP_DOWN)
                uv.w = uv.y + (uv.w - uv.y) * m_AdjustOuterUVScale;
			return uv;
		}

		Vector4 GetAdjustBorder (Vector4 border)
		{
			if(m_dir == Dir.LEFT_RIGHT)
                border.z = border.z - (overrideSprite.rect.width - m_RealAppend) * 0.5f - m_RealAppend;
			else if(m_dir == Dir.UP_DOWN)
                border.w = border.w - (overrideSprite.rect.height - m_RealAppend) * 0.5f - m_RealAppend;
			return border;
		}

		Vector4 GetAlphaOuterUV(Vector4 uv)
		{
            float appendPixelUV = 0;
            
            if (m_dir == Dir.LEFT_RIGHT)
            {
                appendPixelUV = m_AppendPixelScale * (uv.z - uv.x);
                return new Vector4(uv.z + appendPixelUV, uv.y, uv.z + uv.z - uv.x + appendPixelUV, uv.w);
            }
            else if (m_dir == Dir.UP_DOWN)
            {
                appendPixelUV = m_AppendPixelScale * (uv.w - uv.y);
                //Debug.LogError("appendPixelUV : " + appendPixelUV);
                return new Vector4(uv.x, uv.w + appendPixelUV, uv.z, uv.w + uv.w - uv.y + appendPixelUV);
            }
            else
                return Vector4.zero;
		}

		Vector4 GetAlphaInnerUV(Vector4 outerUV, Vector4 innerUV)
		{
			if(m_dir == Dir.LEFT_RIGHT)
			{
                float offset = outerUV.z - outerUV.x + m_AppendPixelScale * (outerUV.z - outerUV.x);
				return new Vector4 (innerUV.x + offset, innerUV.y, innerUV.z + offset, innerUV.w);
			}
			else if(m_dir == Dir.UP_DOWN)
			{
                float offset = outerUV.w - outerUV.y + m_AppendPixelScale * (outerUV.w - outerUV.y);
				return new Vector4(innerUV.x, innerUV.y + offset, innerUV.z, innerUV.w + offset);
			}
			else
				return Vector4.zero;
		}

		#region Various fill functions
		/// <summary>
		/// Generate vertices for a simple Image.
		/// </summary>

		void GenerateSimpleSprite(List<UIVertex> vbo, bool preserveAspect)
		{
			var vert = UIVertex.simpleVert;
			vert.color = color;
			
			Vector4 v = GetDrawingDimensions(preserveAspect);
            var uv = m_PreUV0;
            var uv1 = m_PreUV1;
            if (m_NeedInit)
            {
                uv = (overrideSprite != null) ? GetAdjustOuterUV(DataUtility.GetOuterUV(overrideSprite)) : Vector4.zero;
                uv1 = (overrideSprite != null) ? GetAlphaOuterUV(uv) : Vector4.zero;
                m_PreUV0 = uv;
                m_PreUV1 = uv1;
            }

			vert.position = new Vector3(v.x, v.y);
			vert.uv0 = new Vector2(uv.x, uv.y);
			vert.uv1 = new Vector2 (uv1.x, uv1.y);
			vbo.Add(vert);
			
			vert.position = new Vector3(v.x, v.w);
			vert.uv0 = new Vector2(uv.x, uv.w);
			vert.uv1 = new Vector2 (uv1.x, uv1.w);
			vbo.Add(vert);
			
			vert.position = new Vector3(v.z, v.w);
			vert.uv0 = new Vector2(uv.z, uv.w);
			vert.uv1 = new Vector2 (uv1.z, uv1.w);
			vbo.Add(vert);
			
			vert.position = new Vector3(v.z, v.y);
			vert.uv0 = new Vector2(uv.z, uv.y);
			vert.uv1 = new Vector2 (uv1.z, uv1.y);
			vbo.Add(vert);
		}

		/// <summary>
		/// Generate vertices for a 9-sliced Image.
		/// </summary>
		
		static readonly Vector2[] s_VertScratch = new Vector2[4];
		static readonly Vector2[] s_UVScratch = new Vector2[4];
		static readonly Vector2[] s_UVScratch1 = new Vector2[4];
        private Vector4 m_PreSlicedOuter, m_PreSlicedBoreder, m_PreSlicedOuter1, m_PreSlicedInner1;
		void GenerateSlicedSprite(List<UIVertex> vbo)
		{
			if (!hasBorder)
			{
				GenerateSimpleSprite(vbo, false);
				return;
			}
			
			Vector4 outer, inner, padding, border;
			Vector4 outer1, inner1;
			
			if (overrideSprite != null)
			{
			    if (m_NeedInit)
			    {
                    m_PreSlicedOuter = GetAdjustOuterUV(DataUtility.GetOuterUV(overrideSprite));
                    m_PreSlicedBoreder = GetAdjustBorder(overrideSprite.border);
			    }
			    outer = m_PreSlicedOuter;
				inner = DataUtility.GetInnerUV(overrideSprite);
				padding = DataUtility.GetPadding(overrideSprite);
			    border = m_PreSlicedBoreder;
				if(m_dir == Dir.LEFT_RIGHT)
				{
					if(border.z <= 0)
					{
						border.z = 0;
						inner.z = outer.z;
					}
				}
				else if(m_dir == Dir.UP_DOWN)
				{
					if(border.w <= 0)
					{
						border.w = 0;
						inner.w = outer.w;
					}
				}
			    if (m_NeedInit)
			    {
                    m_PreSlicedOuter1 = GetAlphaOuterUV(outer);
                    m_PreSlicedInner1 =  GetAlphaInnerUV (outer, inner);
			    }
			    outer1 = m_PreSlicedOuter1;
			    inner1 = m_PreSlicedInner1;
			}
			else
			{
				outer = Vector4.zero;
				inner = Vector4.zero;
				padding = Vector4.zero;
				border = Vector4.zero;

				outer1 = Vector4.zero;
				inner1 = Vector4.zero;
			}

			//rect transform
			Rect rect = GetPixelAdjustedRect();

			///rgb sprite//////
			border = GetAdjustedBorders(border / pixelsPerUnit, rect);
			padding = padding / pixelsPerUnit;
			
			s_VertScratch[0] = new Vector2(padding.x, padding.y);
			s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);
			
			s_VertScratch[1].x = border.x;
			s_VertScratch[1].y = border.y;
			s_VertScratch[2].x = rect.width - border.z;
			s_VertScratch[2].y = rect.height - border.w;
			
			for (int i = 0; i < 4; ++i)
			{
				s_VertScratch[i].x += rect.x;
				s_VertScratch[i].y += rect.y;
			}

			///rgb sprite
			s_UVScratch[0] = new Vector2(outer.x, outer.y);
			s_UVScratch[1] = new Vector2(inner.x, inner.y);
			s_UVScratch[2] = new Vector2(inner.z, inner.w);
			s_UVScratch[3] = new Vector2(outer.z, outer.w);

			///alpha sprite
			s_UVScratch1[0] = new Vector2(outer1.x, outer1.y);
			s_UVScratch1[1] = new Vector2(inner1.x, inner1.y);
			s_UVScratch1[2] = new Vector2(inner1.z, inner1.w);
			s_UVScratch1[3] = new Vector2(outer1.z, outer1.w);
			
			var uiv = UIVertex.simpleVert;
			uiv.color = color;
			for (int x = 0; x < 3; ++x)
			{
				int x2 = x + 1;
				
				for (int y = 0; y < 3; ++y)
				{
					if (!fillCenter && x == 1 && y == 1)
					{
						continue;
					}
					
					int y2 = y + 1;
					
					AddQuad(vbo, uiv,
					        new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
					        new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
					        new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
					        new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y),
					        new Vector2(s_UVScratch1[x].x, s_UVScratch1[y].y),
					        new Vector2(s_UVScratch1[x2].x, s_UVScratch1[y2].y));
				}
			}
		}

	    private Vector4 m_PreTiledOuter, m_PreTiledBorder, m_PreTiledOuter1, m_PreTiledInner1;
		/// <summary>
		/// Generate vertices for a tiled Image.
		/// </summary>
		
		void GenerateTiledSprite(List<UIVertex> vbo)
		{
			Vector4 outer, inner, border;
			Vector4 outer1, inner1;
			Vector2 spriteSize;
			
			if (overrideSprite != null)
			{
			    if (m_NeedInit)
			    {
                    m_PreTiledOuter = GetAdjustOuterUV(DataUtility.GetOuterUV(overrideSprite));
                    m_PreTiledBorder = GetAdjustBorder(overrideSprite.border);
			    }
			    outer = m_PreTiledOuter;
				inner = DataUtility.GetInnerUV(overrideSprite);
			    border = m_PreTiledBorder;
				spriteSize = overrideSprite.rect.size;
				if(m_dir == Dir.LEFT_RIGHT)
				{
					if(border.z <= 0)
					{
						border.z = 0;
						inner.z = outer.z;
					}
					spriteSize.x = spriteSize.x * 0.5f;
				}
				else if(m_dir == Dir.UP_DOWN)
				{
					if(border.w <= 0)
					{
						border.w = 0;
						inner.w = outer.w;
					}
					spriteSize.y = spriteSize.y * 0.5f;
				}
                if (m_NeedInit)
                {
                    m_PreTiledOuter1 = GetAlphaOuterUV(outer);
                    m_PreTiledInner1 = GetAlphaInnerUV (outer, inner);
                }
			    outer1 = m_PreTiledOuter1;
			    inner1 = m_PreTiledInner1;
			}
			else
			{
				outer = Vector4.zero;
				inner = Vector4.zero;
				border = Vector4.zero;
				spriteSize = Vector2.one * 100;

				outer1 = Vector4.zero;
				inner1 = Vector4.zero;
			}
			
			Rect rect = GetPixelAdjustedRect();
			float tileWidth = (spriteSize.x - border.x - border.z) / pixelsPerUnit;
			float tileHeight = (spriteSize.y - border.y - border.w) / pixelsPerUnit;
			border = GetAdjustedBorders(border / pixelsPerUnit, rect);
			
			var uvMin = new Vector2(inner.x, inner.y);
			var uvMax = new Vector2(inner.z, inner.w);

			///alpha sprite
			var uvMin1 = new Vector2 (inner1.x, inner1.y);
			var uvMax1 = new Vector2 (inner1.z, inner1.w);
			
			var v = UIVertex.simpleVert;
			v.color = color;
			
			// Min to max max range for tiled region in coordinates relative to lower left corner.
			float xMin = border.x;
			float xMax = rect.width - border.z;
			float yMin = border.y;
			float yMax = rect.height - border.w;
			
			// Safety check. Useful so Unity doesn't run out of memory if the sprites are too small.
			// Max tiles are 100 x 100.
			if ((xMax - xMin) > tileWidth * 100 || (yMax - yMin) > tileHeight * 100)
			{
				tileWidth = (xMax - xMin) / 100;
				tileHeight = (yMax - yMin) / 100;
			}
			
			var clipped = uvMax;
			var clipped1 = uvMax1;
			if (fillCenter)
			{
				for (float y1 = yMin; y1 < yMax; y1 += tileHeight)
				{
					float y2 = y1 + tileHeight;
					if (y2 > yMax)
					{
						clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
						clipped1.y = uvMin1.y + (uvMax1.y - uvMin1.y) * (yMax - y1) / (y2 - y1);
						y2 = yMax;
					}
					
					clipped.x = uvMax.x;
					clipped1.x = uvMax1.x;
					for (float x1 = xMin; x1 < xMax; x1 += tileWidth)
					{
						float x2 = x1 + tileWidth;
						if (x2 > xMax)
						{
							clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
							clipped1.x = uvMin1.x + (uvMax1.x - uvMin1.x) * (xMax - x1) / (x2 - x1);
							x2 = xMax;
						}
						AddQuad(vbo, v, new Vector2(x1, y1) + rect.position, new Vector2(x2, y2) + rect.position, uvMin, clipped, uvMin1, clipped1);
					}
				}
			}
			
			if (!hasBorder)
				return;
			
			// Left and right tiled border
			clipped = uvMax;
			clipped1 = uvMax1;
			for (float y1 = yMin; y1 < yMax; y1 += tileHeight)
			{
				float y2 = y1 + tileHeight;
				if (y2 > yMax)
				{
					clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
					clipped1.y = uvMin1.y + (uvMax1.y - uvMin1.y) * (yMax - y1) / (y2 - y1);
					y2 = yMax;
				}
				AddQuad(vbo, v,
				        new Vector2(0, y1) + rect.position,
				        new Vector2(xMin, y2) + rect.position,
				        new Vector2(outer.x, uvMin.y),
				        new Vector2(uvMin.x, clipped.y),
				        new Vector2(outer1.x, uvMin1.y),
				        new Vector2(uvMin1.x, clipped1.y));
				AddQuad(vbo, v,
				        new Vector2(xMax, y1) + rect.position,
				        new Vector2(rect.width, y2) + rect.position,
				        new Vector2(uvMax.x, uvMin.y),
				        new Vector2(outer.z, clipped.y),
				        new Vector2(uvMax1.x, uvMin1.y),
				        new Vector2(outer1.z, clipped1.y));
			}
			
			// Bottom and top tiled border
			clipped = uvMax;
			clipped1 = uvMax1;
			for (float x1 = xMin; x1 < xMax; x1 += tileWidth)
			{
				float x2 = x1 + tileWidth;
				if (x2 > xMax)
				{
					clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
					clipped1.x = uvMin1.x + (uvMax1.x - uvMin1.x) * (xMax - x1) / (x2 - x1);
					x2 = xMax;
				}
				AddQuad(vbo, v,
				        new Vector2(x1, 0) + rect.position,
				        new Vector2(x2, yMin) + rect.position,
				        new Vector2(uvMin.x, outer.y),
				        new Vector2(clipped.x, uvMin.y),
				        new Vector2(uvMin1.x, outer1.y),
				        new Vector2(clipped1.x, uvMin1.y));
				AddQuad(vbo, v,
				        new Vector2(x1, yMax) + rect.position,
				        new Vector2(x2, rect.height) + rect.position,
				        new Vector2(uvMin.x, uvMax.y),
				        new Vector2(clipped.x, outer.w),
				        new Vector2(uvMin1.x, uvMax1.y),
				        new Vector2(clipped1.x, outer1.w));
			}
			
			// Corners
			AddQuad(vbo, v,
			        new Vector2(0, 0) + rect.position,
			        new Vector2(xMin, yMin) + rect.position,
			        new Vector2(outer.x, outer.y),
			        new Vector2(uvMin.x, uvMin.y),
			        new Vector2(outer1.x, outer1.y),
			        new Vector2(uvMin1.x, uvMin1.y));
			AddQuad(vbo, v,
			        new Vector2(xMax, 0) + rect.position,
			        new Vector2(rect.width, yMin) + rect.position,
			        new Vector2(uvMax.x, outer.y),
			        new Vector2(outer.z, uvMin.y),
			        new Vector2(uvMax1.x, outer1.y),
			        new Vector2(outer1.z, uvMin1.y));
			AddQuad(vbo, v,
			        new Vector2(0, yMax) + rect.position,
			        new Vector2(xMin, rect.height) + rect.position,
			        new Vector2(outer.x, uvMax.y),
			        new Vector2(uvMin.x, outer.w),
			        new Vector2(outer1.x, uvMax1.y),
			        new Vector2(uvMin1.x, outer1.w));
			AddQuad(vbo, v,
			        new Vector2(xMax, yMax) + rect.position,
			        new Vector2(rect.width, rect.height) + rect.position,
			        new Vector2(uvMax.x, uvMax.y),
			        new Vector2(outer.z, outer.w),
			        new Vector2(uvMax1.x, uvMax1.y),
			        new Vector2(outer1.z, outer1.w));
		}


	    /// <summary>
	    /// Generate vertices for a filled Image.
	    /// </summary>
	    private Vector4 m_PreFilledOuter, m_PreFilledOuter1;
		static readonly Vector2[] s_Xy = new Vector2[4];
		static readonly Vector2[] s_Uv = new Vector2[4];
		static readonly Vector2[] s_Uv1 = new Vector2[4];
		void GenerateFilledSprite(List<UIVertex> vbo, bool preserveAspect)
		{
            if (m_NeedInit)
            {
		        m_PreFilledOuter =  overrideSprite != null ? GetAdjustOuterUV(DataUtility.GetOuterUV(overrideSprite)) : Vector4.zero;
                m_PreFilledOuter1= GetAlphaOuterUV (m_PreFilledOuter);
            }
			if (fillAmount < 0.001f)
				return;
			Vector4 v = GetDrawingDimensions(preserveAspect);
		    Vector4 outer = m_PreFilledOuter;
		    Vector4 outer1 = m_PreFilledOuter1;
			UIVertex uiv = UIVertex.simpleVert;
			uiv.color = color;
			
			float tx0 = outer.x;
			float ty0 = outer.y;
			float tx1 = outer.z;
			float ty1 = outer.w;

			//alpha sprite
			float txa0 = outer1.x;
			float tya0 = outer1.y;
			float txa1 = outer1.z;
			float tya1 = outer1.w;
			
			// Horizontal and vertical filled sprites are simple -- just end the Image prematurely
			if (fillMethod == FillMethod.Horizontal || fillMethod == FillMethod.Vertical)
			{
				if (fillMethod == FillMethod.Horizontal)
				{
					float fill = (tx1 - tx0) * fillAmount;
					float fill1 = (txa1 - txa0) * fillAmount;
					
					if (fillOrigin == 1)
					{
						v.x = v.z - (v.z - v.x) * fillAmount;
						tx0 = tx1 - fill;
						txa0 = txa1 - fill1;
					}
					else
					{
						v.z = v.x + (v.z - v.x) * fillAmount;
						tx1 = tx0 + fill;
						txa1 = txa0 + fill1;
					}
				}
				else if (fillMethod == FillMethod.Vertical)
				{
					float fill = (ty1 - ty0) * fillAmount;
					float fill1 = (tya1 - tya0) * fillAmount;
					
					if (fillOrigin == 1)
					{
						v.y = v.w - (v.w - v.y) * fillAmount;
						ty0 = ty1 - fill;
						tya0 = tya1 - fill1;
					}
					else
					{
						v.w = v.y + (v.w - v.y) * fillAmount;
						ty1 = ty0 + fill;
						tya1 = tya0 + fill1;
					}
				}
			}
			
			s_Xy[0] = new Vector2(v.x, v.y);
			s_Xy[1] = new Vector2(v.x, v.w);
			s_Xy[2] = new Vector2(v.z, v.w);
			s_Xy[3] = new Vector2(v.z, v.y);
			
			s_Uv[0] = new Vector2(tx0, ty0);
			s_Uv[1] = new Vector2(tx0, ty1);
			s_Uv[2] = new Vector2(tx1, ty1);
			s_Uv[3] = new Vector2(tx1, ty0);

			//alpha sprite
			s_Uv1[0] = new Vector2(txa0, tya0);
			s_Uv1[1] = new Vector2(txa0, tya1);
			s_Uv1[2] = new Vector2(txa1, tya1);
			s_Uv1[3] = new Vector2(txa1, tya0);
			
			if (fillAmount < 1f)
			{
				if (fillMethod == FillMethod.Radial90)
				{
					if (RadialCut(s_Xy, s_Uv, fillAmount, fillClockwise, fillOrigin, s_Uv1))
					{
						for (int i = 0; i < 4; ++i)
						{
							uiv.position = s_Xy[i];
							uiv.uv0 = s_Uv[i];
							uiv.uv1 = s_Uv1[i];
							vbo.Add(uiv);
						}
					}
					return;
				}
				
				if (fillMethod == FillMethod.Radial180)
				{
					for (int side = 0; side < 2; ++side)
					{
						float fx0, fx1, fy0, fy1;
						int even = fillOrigin > 1 ? 1 : 0;
						
						if (fillOrigin == 0 || fillOrigin == 2)
						{
							fy0 = 0f;
							fy1 = 1f;
							if (side == even) { fx0 = 0f; fx1 = 0.5f; }
							else { fx0 = 0.5f; fx1 = 1f; }
						}
						else
						{
							fx0 = 0f;
							fx1 = 1f;
							if (side == even) { fy0 = 0.5f; fy1 = 1f; }
							else { fy0 = 0f; fy1 = 0.5f; }
						}
						
						s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
						s_Xy[1].x = s_Xy[0].x;
						s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
						s_Xy[3].x = s_Xy[2].x;
						
						s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
						s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
						s_Xy[2].y = s_Xy[1].y;
						s_Xy[3].y = s_Xy[0].y;
						
						s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
						s_Uv[1].x = s_Uv[0].x;
						s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
						s_Uv[3].x = s_Uv[2].x;
						
						s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
						s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
						s_Uv[2].y = s_Uv[1].y;
						s_Uv[3].y = s_Uv[0].y;

						//alpha sprite
						s_Uv1[0].x = Mathf.Lerp(txa0, txa1, fx0);
						s_Uv1[1].x = s_Uv1[0].x;
						s_Uv1[2].x = Mathf.Lerp(txa0, txa1, fx1);
						s_Uv1[3].x = s_Uv1[2].x;
						
						s_Uv1[0].y = Mathf.Lerp(tya0, tya1, fy0);
						s_Uv1[1].y = Mathf.Lerp(tya0, tya1, fy1);
						s_Uv1[2].y = s_Uv1[1].y;
						s_Uv1[3].y = s_Uv1[0].y;
						
						float val = fillClockwise ? fillAmount * 2f - side : fillAmount * 2f - (1 - side);
						
						if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), fillClockwise, ((side + fillOrigin + 3) % 4), s_Uv1))
						{
							for (int i = 0; i < 4; ++i)
							{
								uiv.position = s_Xy[i];
								uiv.uv0 = s_Uv[i];
								uiv.uv1 = s_Uv1[i];
								vbo.Add(uiv);
							}
						}
					}
					return;
				}
				
				if (fillMethod == FillMethod.Radial360)
				{
					for (int corner = 0; corner < 4; ++corner)
					{
						float fx0, fx1, fy0, fy1;
						
						if (corner < 2) { fx0 = 0f; fx1 = 0.5f; }
						else { fx0 = 0.5f; fx1 = 1f; }
						
						if (corner == 0 || corner == 3) { fy0 = 0f; fy1 = 0.5f; }
						else { fy0 = 0.5f; fy1 = 1f; }
						
						s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
						s_Xy[1].x = s_Xy[0].x;
						s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
						s_Xy[3].x = s_Xy[2].x;
						
						s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
						s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
						s_Xy[2].y = s_Xy[1].y;
						s_Xy[3].y = s_Xy[0].y;
						
						s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
						s_Uv[1].x = s_Uv[0].x;
						s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
						s_Uv[3].x = s_Uv[2].x;
						
						s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
						s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
						s_Uv[2].y = s_Uv[1].y;
						s_Uv[3].y = s_Uv[0].y;

						//alpha sprite
						s_Uv1[0].x = Mathf.Lerp(txa0, txa1, fx0);
						s_Uv1[1].x = s_Uv1[0].x;
						s_Uv1[2].x = Mathf.Lerp(txa0, txa1, fx1);
						s_Uv1[3].x = s_Uv1[2].x;
						
						s_Uv1[0].y = Mathf.Lerp(tya0, tya1, fy0);
						s_Uv1[1].y = Mathf.Lerp(tya0, tya1, fy1);
						s_Uv1[2].y = s_Uv1[1].y;
						s_Uv1[3].y = s_Uv1[0].y;
						
						float val = fillClockwise ?
							fillAmount * 4f - ((corner + fillOrigin) % 4) :
								fillAmount * 4f - (3 - ((corner + fillOrigin) % 4));
						
						if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), fillClockwise, ((corner + 2) % 4), s_Uv1))
						{
							for (int i = 0; i < 4; ++i)
							{
								uiv.position = s_Xy[i];
								uiv.uv0 = s_Uv[i];
								uiv.uv1 = s_Uv1[i];
								vbo.Add(uiv);
							}
						}
					}
					return;
				}
			}
			
			// Fill the buffer with the quad for the Image
			for (int i = 0; i < 4; ++i)
			{
				uiv.position = s_Xy[i];
				uiv.uv0 = s_Uv[i];
				uiv.uv1 = s_Uv1[i];
				vbo.Add(uiv);
			}
		}

		/// <summary>
		/// Adjust the specified quad, making it be radially filled instead.
		/// </summary>
		
		static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner, Vector2[] uv1 = null)
		{
			// Nothing to fill
			if (fill < 0.001f) return false;
			
			// Even corners invert the fill direction
			if ((corner & 1) == 1) invert = !invert;
			
			// Nothing to adjust
			if (!invert && fill > 0.999f) return true;
			
			// Convert 0-1 value into 0 to 90 degrees angle in radians
			float angle = Mathf.Clamp01(fill);
			if (invert) angle = 1f - angle;
			angle *= 90f * Mathf.Deg2Rad;
			
			// Calculate the effective X and Y factors
			float cos = Mathf.Cos(angle);
			float sin = Mathf.Sin(angle);
			
			RadialCut(xy, cos, sin, invert, corner);
			RadialCut(uv, cos, sin, invert, corner, uv1);
			return true;
		}
		
		/// <summary>
		/// Adjust the specified quad, making it be radially filled instead.
		/// </summary>
		
		static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner, Vector2[] xy1 = null)
		{
			int i0 = corner;
			int i1 = ((corner + 1) % 4);
			int i2 = ((corner + 2) % 4);
			int i3 = ((corner + 3) % 4);
			
			if ((corner & 1) == 1)
			{
				if (sin > cos)
				{
					cos /= sin;
					sin = 1f;
					
					if (invert)
					{
						xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
						xy[i2].x = xy[i1].x;
						if(xy1 != null)
						{
							xy1[i1].x = Mathf.Lerp(xy1[i0].x, xy1[i2].x, cos);
							xy1[i2].x = xy1[i1].x;
						}
					}
				}
				else if (cos > sin)
				{
					sin /= cos;
					cos = 1f;
					
					if (!invert)
					{
						xy[i2].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
						xy[i3].y = xy[i2].y;
						if(xy1 != null)
						{
							xy1[i2].y = Mathf.Lerp(xy1[i0].y, xy1[i2].y, sin);
							xy1[i3].y = xy1[i2].y;
						}
					}
				}
				else
				{
					cos = 1f;
					sin = 1f;
				}
				
				if (!invert) 
				{	
					xy[i3].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
					if(xy1 != null)
					{
						xy1[i3].x = Mathf.Lerp(xy1[i0].x, xy1[i2].x, cos);
					}
				}
				else 
				{
					xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
					if(xy1 != null)
					{
						xy1[i1].y = Mathf.Lerp(xy1[i0].y, xy1[i2].y, sin);
					}
				}
			}
			else
			{
				if (cos > sin)
				{
					sin /= cos;
					cos = 1f;
					
					if (!invert)
					{
						xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
						xy[i2].y = xy[i1].y;
						if(xy1 != null)
						{
							xy1[i1].y = Mathf.Lerp(xy1[i0].y, xy1[i2].y, sin);
							xy1[i2].y = xy1[i1].y;
						}
					}
				}
				else if (sin > cos)
				{
					cos /= sin;
					sin = 1f;
					
					if (invert)
					{
						xy[i2].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
						xy[i3].x = xy[i2].x;
						if(xy1 != null)
						{
							xy1[i2].x = Mathf.Lerp(xy1[i0].x, xy1[i2].x, cos);
							xy1[i3].x = xy1[i2].x;
						}
					}
				}
				else
				{
					cos = 1f;
					sin = 1f;
				}
				
				if (invert)
				{
					xy[i3].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
					if(xy1 != null)
					{
						xy1[i3].y = Mathf.Lerp(xy1[i0].y, xy1[i2].y, sin);
					}
				}
				else 
				{
					xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
					if(xy1 != null)
					{
						xy1[i1].x = Mathf.Lerp(xy1[i0].x, xy1[i2].x, cos);
					}
				}
			}
		}

		#endregion
	}
}
