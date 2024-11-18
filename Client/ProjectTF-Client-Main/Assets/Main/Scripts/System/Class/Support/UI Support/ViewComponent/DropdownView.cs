using System;
using System.Linq;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace TF.System.UI
{
	[Serializable, InlineProperty, HideLabel]
	public class DropdownView<TEnum> : UIViewItem<TEnum> where TEnum : Enum
	{
		[SerializeField, HorizontalGroup, LabelText("Menu"), LabelWidth(40)]
		private TMP_Dropdown dropdown;
		[SerializeField, HorizontalGroup, LabelText(" Icon"), LabelWidth(40)]
		private Image icon;
		[SerializeField, HorizontalGroup, LabelText(" Info"), LabelWidth(40)]
		private TMP_Text info;
		[Serializable]
		private struct DropdownItem
		{
			public TEnum type;
			public string menu;
			public Sprite icon;
			public string info;
		}

		[SerializeField,TableList(AlwaysExpanded = true, ShowPaging = false )]
		private DropdownItem[] items;

		[SerializeField, EnumPaging]
		private TEnum initValue;

		public bool interaction = true;
		public Action<TEnum> onValueChanged;

#if UNITY_EDITOR
		[Title("Init Preview")]
		[PreviewField(Alignment = ObjectFieldAlignment.Center), ShowInInspector, HideLabel]
		public Sprite PreviewIcon => items.Where(i => i.type.Equals(initValue)).Select(t => t.icon).FirstOrDefault();
		[DisplayAsString(Alignment = TextAlignment.Center), ShowInInspector, HideLabel, EnableGUI]
		public string PreviewText =>
			$"{items.Where(i => i.type.Equals(initValue)).Select(t => t.menu).FirstOrDefault()}" +
			$" | {items.Where(i => i.type.Equals(initValue)).Select(t => t.info).FirstOrDefault()}";
#endif

		protected override void InitView()
		{
			dropdown.ClearOptions();
			dropdown.AddOptions(items.Select(i => i.menu).ToList());

			dropdown.onValueChanged.RemoveAllListeners();
			dropdown.onValueChanged.AddListener(index => {
				if(!interaction) return;

				var item = items[index];
				if(icon) icon.sprite = item.icon;
				if(info) info.text = item.info;
				onValueChanged?.Invoke(item.type);
			});
		}
		public override void ResetValue()
		{
			SetValue(initValue);
		}
		public override TEnum GetValue()
		{
			int value = dropdown.value;
			return items[value].type;
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
				int length = items.Length;
				for(int i = 0 ; i < length ; i++)
				{
					if(items[i].type.Equals(setValue))
					{
						dropdown.value = i;
						return;
					}
				}
				dropdown.value = 0;
			}
		}
	}
}
