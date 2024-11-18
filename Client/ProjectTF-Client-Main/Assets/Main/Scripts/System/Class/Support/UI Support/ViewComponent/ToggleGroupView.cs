using System;
using System.Linq;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace TF.System.UI
{
	[Serializable, InlineProperty, HideLabel]
	public class ToggleGroupView<TEnum> : UIViewItem<TEnum> where TEnum : Enum
	{
		[SerializeField, HorizontalGroup, LabelText("Group"), LabelWidth(40)]
		private ToggleGroup toggleGroup;
		[SerializeField, HorizontalGroup, LabelText(" Icon"), LabelWidth(40)]
		private Image toggleIcon;
		[SerializeField, HorizontalGroup, LabelText(" Info"), LabelWidth(40)]
		private TMP_Text toggleInfo;
		[Serializable]
		private struct ToggleState
		{
			public TEnum type;
			public Toggle toggle;
			public Sprite icon;
			public string info;
		}
		[SerializeField, TableList]
		private ToggleState[] toggles;

		[SerializeField, EnumPaging]
		private TEnum initValue;

		public bool interaction = true;
		public Action<TEnum> onValueChanged;

#if UNITY_EDITOR
		[Title("Init Preview")]
		[PreviewField(Alignment = ObjectFieldAlignment.Center), ShowInInspector, HideLabel]
		public Sprite PreviewIcon => toggles.Where(i => i.type.Equals(initValue)).Select(t => t.icon).FirstOrDefault();
		[DisplayAsString(Alignment = TextAlignment.Center), ShowInInspector, HideLabel, EnableGUI]
		public string PreviewText => toggles.Where(i => i.type.Equals(initValue)).Select(t => t.info).FirstOrDefault();
#endif
		protected override void InitView()
		{

			int length = toggles.Length;
			for(int i = 0 ; i < length ; i++)
			{
				var state = toggles[i];
				state.toggle.group = toggleGroup;
			}
			toggleGroup.allowSwitchOff = false;

			for(int i = 0 ; i < length ; i++)
			{
				var state = toggles[i];
				state.toggle.onValueChanged.RemoveAllListeners();
				state.toggle.onValueChanged.AddListener(isOn => {
					if(!interaction) return;

					if(isOn)
					{
						if(toggleIcon) toggleIcon.sprite = state.icon;
						if(toggleInfo) toggleInfo.text = state.info;

						onValueChanged?.Invoke(state.type);
					}
				});
			}
		}
		public override void ResetValue()
		{
			SetValue(initValue);
		}

		public override TEnum GetValue()
		{
			int length = toggles.Length;
			for(int i = 0 ; i < length ; i++)
			{
				if(toggles[i].toggle.isOn)
				{
					return toggles[i].type;
				}
			}
			return default;
		}
		public override void SetValue(TEnum setValue, bool _interaction = true)
		{
			if(_interaction)
			{
				Set();
			}
			else
			{
				var old = interaction;
				interaction = false;
				Set();
				interaction = old;
			}
			void Set()
			{
				int length = toggles.Length;
				for(int i = 0 ; i < length ; i++)
				{
					var state = toggles[i];
					state.toggle.isOn = state.type.Equals(setValue);
				}
			}
		}
	}
}
