#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace TFContent
{
	[RequireComponent(typeof(Button))]
	public abstract class Editor_Button : MonoBehaviour
    {
		protected abstract void OnClick();

		protected void Awake()
		{
			Button button = GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(OnClick);
		}
	}
}
#endif