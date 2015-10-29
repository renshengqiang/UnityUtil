using UnityEngine;
using System.Collections;

namespace Util
{
	public class ExtendBehavior : MonoBehaviour
	{
		private Transform _cachedTranform;
		public Transform cachedTransform
		{
			get{
				if(null == _cachedTranform)
				{
					_cachedTranform = transform;
				}
				return _cachedTranform;
			}
		}
	}
}