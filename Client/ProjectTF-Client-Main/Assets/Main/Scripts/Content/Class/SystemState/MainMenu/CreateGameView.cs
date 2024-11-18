using System;
using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using TF.System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace TF.Content
{
	public class CreateGameView : ComponentBehaviour, IUIShowAndHideControl
	{
		public UIShowAndHide ThisUIShowAndHide { get; set; }


		[SerializeField] private Button cancelButton;
		[SerializeField] private Button helpButton;
		[SerializeField] private Button confirmButton;
		[SerializeField, ReadOnly]
		private bool onClick;

		#region View Item Setting
		public class ViewSetup
		{
			public void Init() { InitView(); ResetValue(); }
			protected virtual void InitView() { }
			public virtual void ResetValue() { }
		}
		[FoldoutGroup("Room Title")]
		public TextInputField roomTitle;
		[Serializable, InlineProperty, HideLabel]
		public class TextInputField : ViewSetup
		{
			public TMP_InputField titleInput;
			[LabelText("Ό³ΈνΉ®"), LabelWidth(50), Multiline(2)]
			public string placeholderText;
			public int characterLimit = 25;
			protected override void InitView()
			{
				titleInput.characterLimit = 25;
				titleInput.lineLimit = 0;
				var placeholder = titleInput.placeholder;
				if(placeholder != null && placeholder.TryGetComponent<TMP_Text>(out var tmp_Text))
				{
					tmp_Text.text = placeholderText;
				}
			}
			public override void ResetValue()
			{
				titleInput.text = "";
			}
			public string GetTitle()
			{
				return titleInput.text;
			}
		}

		[FoldoutGroup("Room Public Type")]
		public ToggleGroupView<RoomPublicType> roomPublicType;
		public enum RoomPublicType
		{
			Public, Private
		}
		[Serializable, InlineProperty, HideLabel]
		public class ToggleGroupView<TEnum> : ViewSetup where TEnum : Enum
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
				public TEnum @enum;
				public Toggle toggle;
				public Sprite icon;
				public string info;
			}
			[SerializeField, TableList]
			private List<ToggleState> toggles;

			[SerializeField, EnumPaging]
			private TEnum initValue;

#if UNITY_EDITOR
			[Title("Init Preview")]
			[PreviewField(Alignment = ObjectFieldAlignment.Center), ShowInInspector, HideLabel]
			public Sprite PreviewIcon => toggles.Where(i => i.@enum.Equals(initValue)).Select(t => t.icon).FirstOrDefault();
			[DisplayAsString(Alignment = TextAlignment.Center), ShowInInspector, HideLabel, EnableGUI]
			public string PreviewText => toggles.Where(i => i.@enum.Equals(initValue)).Select(t => t.info).FirstOrDefault();
#endif
			protected override void InitView()
			{
				int length = toggles.Count;
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
						if(isOn)
						{
							if(toggleIcon) toggleIcon.sprite = state.icon;
							if(toggleInfo) toggleInfo.text = state.info;
						}
					});
				}

			}
			public override void ResetValue()
			{
				int length = toggles.Count;
				for(int i = 0 ; i < length ; i++)
				{
					var state = toggles[i];
					state.toggle.isOn = state.@enum.Equals(initValue);
				}
			}

			public TEnum GetOnToggleValue()
			{
				int length = toggles.Count;
				for(int i = 0 ; i < length ; i++)
				{
					if(toggles[i].toggle.isOn)
					{
						return toggles[i].@enum;
					}
				}
				return default;
			}
		}

		[FoldoutGroup("Number Of Player")]
		public DropdownView<NumberOfPlayer> numberOfPlayer;
		public enum NumberOfPlayer
		{
			None = 0,
			Player_1 = 1,
			Player_2 = 2,
			Player_3 = 3,
			Player_4 = 4
		}
		[Serializable, InlineProperty, HideLabel]
		public class DropdownView<TEnum> : ViewSetup where TEnum : Enum
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
				public TEnum @enum;
				public string menu;
				public Sprite icon;
				public string info;
			}

			[SerializeField,TableList]
			private List<DropdownItem> items;

			[SerializeField, EnumPaging]
			private TEnum initValue;

#if UNITY_EDITOR
			[Title("Init Preview")]
			[PreviewField(Alignment = ObjectFieldAlignment.Center), ShowInInspector, HideLabel]
			public Sprite PreviewIcon => items.Where(i => i.@enum.Equals(initValue)).Select(t => t.icon).FirstOrDefault();
			[DisplayAsString(Alignment = TextAlignment.Center), ShowInInspector, HideLabel, EnableGUI]
			public string PreviewText =>
				$"{items.Where(i => i.@enum.Equals(initValue)).Select(t => t.menu).FirstOrDefault()}" +
				$" | {items.Where(i => i.@enum.Equals(initValue)).Select(t => t.info).FirstOrDefault()}";
#endif

			protected override void InitView()
			{
				dropdown.ClearOptions();
				dropdown.AddOptions(items.Select(i => i.menu).ToList());
				dropdown.onValueChanged.RemoveAllListeners();
				dropdown.onValueChanged.AddListener(index => {
					if(icon) icon.sprite = items[index].icon;
					if(info) info.text = items[index].info;
				});
			}
			public override void ResetValue()
			{
				dropdown.value = items.FindIndex(i => i.@enum.Equals(initValue));
			}

			public TEnum GetDropdownValue()
			{
				int value = dropdown.value;
				return items[value].@enum;
			}
		}
		#endregion

		public override void BaseAwake()
		{
			roomTitle.Init();
			roomPublicType.Init();
			numberOfPlayer.Init();

			onClick = false;
			cancelButton.onClick.AddListener(async () => await WaitOnClick(OnCancelButton()));
			helpButton.onClick.AddListener(async () => await WaitOnClick(OnHelpButton()));
			confirmButton.onClick.AddListener(async () => await WaitOnClick(OnConfirmButton()));
			async Awaitable WaitOnClick(Awaitable awaitable)
			{
				if(onClick) return;
				onClick = true;
				await awaitable;
				onClick = false;
			}
		}
		public override void BaseEnable()
		{
			roomTitle.ResetValue();
			roomPublicType.ResetValue();
			numberOfPlayer.ResetValue();
		}

		private async Awaitable OnCancelButton()
		{
			if(ThisContainer.TryGetComponent<MainButtonView>(out var view))
			{
				if(view.ThisContainer.TryGetComponent<IUIShowAndHideControl>(out var viewShowHide, i => i.GameObject == view.GameObject))
				{
					IUIShowAndHideControl thisShowHide = this;

					viewShowHide.GameObject.SetActive(true);
					await AwaitableUtility.ParallelWaitAll(thisShowHide.OnHide(), viewShowHide.OnShow());
					thisShowHide.GameObject.SetActive(false);
				}
			}
		}
		private async Awaitable OnHelpButton()
		{
			if(ThisContainer.TryGetComponent<MainButtonView>(out var view))
			{
				if(view.ThisContainer.TryGetComponent<IUIShowAndHideControl>(out var viewShowHide, i => i.GameObject == view.GameObject))
				{
					IUIShowAndHideControl thisShowHide = this;

					viewShowHide.GameObject.SetActive(true);
					await AwaitableUtility.ParallelWaitAll(thisShowHide.OnHide(), viewShowHide.OnShow());
					thisShowHide.GameObject.SetActive(false);
				}
			}
		}
		private async Awaitable OnConfirmButton()
		{
			if(ThisContainer.TryGetComponent<MainButtonView>(out var view))
			{
				if(view.ThisContainer.TryGetComponent<IUIShowAndHideControl>(out var viewShowHide, i => i.GameObject == view.GameObject))
				{
					IUIShowAndHideControl thisShowHide = this;

					viewShowHide.GameObject.SetActive(true);
					await AwaitableUtility.ParallelWaitAll(thisShowHide.OnHide(), viewShowHide.OnShow());
					thisShowHide.GameObject.SetActive(false);
				}
			}
		}
	}
}
