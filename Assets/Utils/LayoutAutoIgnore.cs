using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Util
{
	public class LayoutAutoIgnore : MonoBehaviour
	{
		public LayoutElement element;

		void OnEnable()
		{
			if (null != element) {
				element.ignoreLayout = false;
			}
		}

		void OnDisable()
		{
			if (null != element) {
				element.ignoreLayout = true;
			}
		}
	}
}