using System;
using System.Collections.Generic;

using BC.Base;

using Sirenix.OdinInspector;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace TF.System.UI
{
	public abstract class UIViewModelComponent : UIViewComponent, IUIViewModel
	{
		private UIShowAndHide thisUIShowAndHide;

		[PropertyOrder(-4), PropertySpace(0, 10), ReadOnly, ShowInInspector]
		public IUIShowAndHide ThisUIShowAndHide {
			get {
				if(thisUIShowAndHide == null)
				{
					thisUIShowAndHide = gameObject.GetComponentInChildren<UIShowAndHide>();
				}
				return thisUIShowAndHide;
			}
		}

		private ViewItemSetter viewItemSetter;
		protected struct ViewItemSetter : IDisposable
		{
			private List<UIViewItem> uiViewItemList;
			internal static ViewItemSetter New() => new ViewItemSetter() {
				uiViewItemList = new List<UIViewItem>()
			};

			public bool HsaItem => uiViewItemList != null && uiViewItemList.Count > 0;
			/// <summary>
			/// 1. 초기화 할 UIViewItem 넣기.<br></br>
			/// 2. nameof(uiViewItem) 를 사용하기.
			/// </summary>
			/// <returns></returns>
			public ViewItemSetter Add(UIViewItem uiViewItem, string nameOfViewItem)
			{
				if(uiViewItem == null || nameOfViewItem.IsNullOrWhiteSpace())
				{
					UnityEngine.Debug.LogError($"UIViewItem({uiViewItem}) 또는 ViewItemName({nameOfViewItem}) 은 null 또는 빈 값을 허용하지 않습니다.");
					return this;
				}
				if(uiViewItemList.Contains(uiViewItem)) return this;
				if(uiViewItemList.FindIndex(i => i.ViewItemName.Equals(nameOfViewItem)) >= 0) return this;

				uiViewItemList.Add(uiViewItem);
				uiViewItem.ViewItemName = nameOfViewItem;

				uiViewItem.Init();
				return this;
			}
			public ViewItemSetter Add<TEnum>(ToggleGroupView<TEnum> uiViewItem, string nameOfViewItem, Action<TEnum> onChangeValue) where TEnum : Enum
			{
				if(uiViewItem == null || nameOfViewItem.IsNullOrWhiteSpace())
				{
					UnityEngine.Debug.LogError($"UIViewItem({uiViewItem}) 또는 ViewItemName({nameOfViewItem}) 은 null 또는 빈 값을 허용하지 않습니다.");
					return this;
				}
				if(uiViewItemList.Contains(uiViewItem)) return this;
				if(uiViewItemList.FindIndex(i => i.ViewItemName.Equals(nameOfViewItem)) >= 0) return this;

				uiViewItemList.Add(uiViewItem);
				uiViewItem.ViewItemName = nameOfViewItem;
				uiViewItem.onValueChanged = onChangeValue;
				uiViewItem.Init();
				return this;
			}
			public ViewItemSetter Add<TEnum>(DropdownView<TEnum> uiViewItem, string nameOfViewItem, Action<TEnum> onChangeValue) where TEnum : Enum
			{
				if(uiViewItem == null || nameOfViewItem.IsNullOrWhiteSpace())
				{
					UnityEngine.Debug.LogError($"UIViewItem({uiViewItem}) 또는 ViewItemName({nameOfViewItem}) 은 null 또는 빈 값을 허용하지 않습니다.");
					return this;
				}
				if(uiViewItemList.Contains(uiViewItem)) return this;
				if(uiViewItemList.FindIndex(i => i.ViewItemName.Equals(nameOfViewItem)) >= 0) return this;

				uiViewItemList.Add(uiViewItem);
				uiViewItem.ViewItemName = nameOfViewItem;
				uiViewItem.onValueChanged = onChangeValue;
				uiViewItem.Init();
				return this;
			}
			public ViewItemSetter Add(TextInputField uiViewItem, string nameOfViewItem, Action<string> onSubmit, Action<string> onChangeValue)
			{
				if(uiViewItem == null || nameOfViewItem.IsNullOrWhiteSpace())
				{
					UnityEngine.Debug.LogError($"UIViewItem({uiViewItem}) 또는 ViewItemName({nameOfViewItem}) 은 null 또는 빈 값을 허용하지 않습니다.");
					return this;
				}
				if(uiViewItemList.Contains(uiViewItem)) return this;
				if(uiViewItemList.FindIndex(i => i.ViewItemName.Equals(nameOfViewItem)) >= 0) return this;

				uiViewItemList.Add(uiViewItem);
				uiViewItem.ViewItemName = nameOfViewItem;
				uiViewItem.onSubmit = onSubmit;
				uiViewItem.onValueChanged = onChangeValue;
				uiViewItem.Init();
				return this;
			}
			public bool TryGetBinding<T>(string nameOfViewItem, out UIBinding<T> viewItem)
			{
				viewItem = null;
				if(!HsaItem) return false;

				if(nameOfViewItem.IsNotNullOrWhiteSpace())
				{
					int length = uiViewItemList.Count;
					for(int i = 0 ; i < length ; i++)
					{
						UIViewItem item = uiViewItemList[i];
						if(item.ViewItemName.Equals(nameOfViewItem) && item is UIBinding<T> tItem)
						{
							viewItem = tItem;
							break;
						}
					}
				}
				return viewItem != null;
			}
			public bool TryGetEventHandle<THandle>(string nameOfViewItem, out THandle eventHandle) where THandle : class, UIEventHandle
			{
				eventHandle = null;
				if(!HsaItem) return false;

				if(nameOfViewItem.IsNotNullOrWhiteSpace())
				{
					int length = uiViewItemList.Count;
					for(int i = 0 ; i < length ; i++)
					{
						UIViewItem item = uiViewItemList[i];
						if(item.ViewItemName.Equals(nameOfViewItem) && item is THandle tItem)
						{
							eventHandle = tItem;
							break;
						}
					}
				}
				return eventHandle != null;
			}
			public void Dispose()
			{
				if(uiViewItemList != null)
				{
					uiViewItemList.Clear();
					uiViewItemList = null;
				}
			}
		}
		protected override void BaseValidate()
		{
			if(ThisUIShowAndHide == null)
			{
				Debug.LogException(new Exception("IUIShowAndHide 컴포넌트가 없습니다. UIViewComponent 에는 최소한 1개의 UIShowAndHide 가 있어야 합니다."));
			}
		}
		sealed protected override void BaseAwake()
		{
			viewItemSetter = ViewItemSetter.New();
			AwakeUIView(ref viewItemSetter);
		}
		sealed protected override async void BaseEnable()
		{
			await OnShowUIView();
		}
		sealed protected override async void BaseDisable()
		{
			await OnHideUIView();
		}
		sealed protected override void BaseDestroy()
		{
			DestroyUIView();
			viewItemSetter.Dispose();
		}

		protected abstract void AwakeUIView(ref ViewItemSetter viewItemSetter);
		protected virtual void DestroyUIView() { }
		protected virtual async Awaitable OnShowUIView() { await ThisUIShowAndHide.OnShow(); }
		protected virtual async Awaitable OnHideUIView() { await ThisUIShowAndHide.OnShow(); }

		public bool TryGetBinding<T>(string nameOfViewItem, out UIBinding<T> viewItem)
		{
			viewItemSetter.TryGetBinding(nameOfViewItem, out viewItem);

			return viewItem != null;
		}

		public bool TryGetEventHandle<THandle>(string nameOfViewItem, out THandle eventHandle) where THandle : class, UIEventHandle
		{
			viewItemSetter.TryGetEventHandle(nameOfViewItem, out eventHandle);

			return eventHandle != null;
		}
	}
}
