﻿using System;
using System.Linq;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace TF.System.UI
{
	[Serializable, InlineProperty, HideLabel]
	public class DropdownView<TEnum> : UIViewItem, UIBinding<TEnum>, UIEvent_OnChangeValue<TEnum> where TEnum : Enum
	{
		[BoxGroup("View")]
		[SerializeField, HorizontalGroup("View/Item"), LabelText("Menu"), LabelWidth(40)]
		private TMP_Dropdown dropdown;
		[SerializeField, HorizontalGroup("View/Item"), LabelText(" Icon"), LabelWidth(40)]
		private Image icon;
		[SerializeField, HorizontalGroup("View/Item"), LabelText(" Info"), LabelWidth(40)]
		private TMP_Text info;
		[Serializable]
		private struct DropdownItem
		{
			[HorizontalGroup("Dropdown"), LabelText("Type"), LabelWidth(40)]
			public TEnum type;
			[HorizontalGroup("Dropdown"), LabelText(" Image"), LabelWidth(45)]
			public Sprite image;
			[HorizontalGroup("Dropdown"), LabelText(" Label"), LabelWidth(45)]
			public string label;
			///////////////////////////////////////////////
			[Header("Sound")]
			[FoldoutGroup("Show Detail"), LabelWidth(100)]
			public string clickSoundPath;
			///////////////////////////////////////////////
			[Title("View Setter")]
			[FoldoutGroup("Show Detail"), LabelWidth(100)]
			public Sprite viewImage;
			[FoldoutGroup("Show Detail"), LabelWidth(100)]
			public string viewText;
		}

		[SerializeField]
		[ListDrawerSettings(ShowPaging = false, ShowFoldout = false, ShowItemCount = false, ShowIndexLabels = false)]
		private DropdownItem[] items;

		[SerializeField, EnumPaging]
		private TEnum initValue;

		public bool interaction { get; set; }
		public Action<TEnum> onValueChanged { get; set; }

#if UNITY_EDITOR
		[Title("Init Preview")]
		[PreviewField(Alignment = ObjectFieldAlignment.Center), ShowInInspector, HideLabel]
		public Sprite PreviewIcon => items == null ? null : items.Where(i => i.type.Equals(initValue)).Select(t => t.viewImage).FirstOrDefault();
		[DisplayAsString(Alignment = TextAlignment.Center), ShowInInspector, HideLabel, EnableGUI]
		public string PreviewText => items == null ? "NULL" :
			$"{items.Where(i => i.type.Equals(initValue)).Select(t => t.label).FirstOrDefault()}" +
			$" | {items.Where(i => i.type.Equals(initValue)).Select(t => t.viewText).FirstOrDefault()}";
#endif

		protected override void InitView()
		{
			interaction = true;
			dropdown.ClearOptions();
			dropdown.AddOptions(items.Select(i => i.label).ToList());

			dropdown.onValueChanged.RemoveAllListeners();
			dropdown.onValueChanged.AddListener(index => {
				if(!interaction) return;

				var item = items[index];
				if(icon) icon.sprite = item.viewImage;
				if(info) info.text = item.label;
				onValueChanged?.Invoke(item.type);
			});
		}
		public override void ResetValue()
		{
			SetValue(initValue);
		}
		public TEnum GetValue()
		{
			int value = dropdown.value;
			return items[value].type;
		}
		public void SetValue(TEnum setValue, bool _interaction = true)
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
