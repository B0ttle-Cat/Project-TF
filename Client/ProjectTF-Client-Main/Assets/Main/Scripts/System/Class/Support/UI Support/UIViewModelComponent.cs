using System;
using System.Collections.Generic;

using BC.Base;

using Sirenix.OdinInspector;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace TF.System.UI
{
	public abstract class UIViewModelComponent : UISupportComponent, IUIViewModel
	{
		[PropertyOrder(-4), PropertySpace(0, 10), ReadOnly, ShowInInspector]
		public IUIShowAndHide ThisUIShowAndHide { get; private set; }

		private ViewItemCollector viewItemCollector;
		protected class ViewItemCollector : IDisposable
		{
			private List<UIViewItem> uiViewItemList;
			internal static ViewItemCollector New() => new ViewItemCollector() {
				uiViewItemList = new List<UIViewItem>()
			};

			/// <summary>
			/// 1. 초기화 할 UIViewItem 넣기.<br></br>
			/// 2. nameof(uiViewItem) 를 사용하기.
			/// </summary>
			/// <returns></returns>
			public ViewItemCollector Add(UIViewItem uiViewItem, string nameOfViewItem)
			{
				if(uiViewItem == null || nameOfViewItem.IsNullOrWhiteSpace())
				{
					UnityEngine.Debug.LogError($"UIViewItem({uiViewItem}) 또는 ViewItemName({nameOfViewItem}) 은 null 또는 빈 값을 허용하지 않습니다.");
					return this;
				}
				if(uiViewItemList.Contains(uiViewItem)) return this;
				if(uiViewItemList.FindIndex(i => i.viewItemName.Equals(nameOfViewItem)) >= 0) return this;

				uiViewItemList.Add(uiViewItem);
				uiViewItem.viewItemName = nameOfViewItem;

				uiViewItem.Init();
				return this;
			}
			public ViewItemCollector Add<TEnum>(ToggleGroupView<TEnum> uiViewItem, string nameOfViewItem, Action<TEnum> onChangeValue) where TEnum : Enum
			{
				if(uiViewItem == null || nameOfViewItem.IsNullOrWhiteSpace())
				{
					UnityEngine.Debug.LogError($"UIViewItem({uiViewItem}) 또는 ViewItemName({nameOfViewItem}) 은 null 또는 빈 값을 허용하지 않습니다.");
					return this;
				}
				if(uiViewItemList.Contains(uiViewItem)) return this;
				if(uiViewItemList.FindIndex(i => i.viewItemName.Equals(nameOfViewItem)) >= 0) return this;

				uiViewItemList.Add(uiViewItem);
				uiViewItem.viewItemName = nameOfViewItem;
				uiViewItem.onValueChanged = onChangeValue;
				uiViewItem.Init();
				return this;
			}
			public ViewItemCollector Add<TEnum>(DropdownView<TEnum> uiViewItem, string nameOfViewItem, Action<TEnum> onChangeValue) where TEnum : Enum
			{
				if(uiViewItem == null || nameOfViewItem.IsNullOrWhiteSpace())
				{
					UnityEngine.Debug.LogError($"UIViewItem({uiViewItem}) 또는 ViewItemName({nameOfViewItem}) 은 null 또는 빈 값을 허용하지 않습니다.");
					return this;
				}
				if(uiViewItemList.Contains(uiViewItem)) return this;
				if(uiViewItemList.FindIndex(i => i.viewItemName.Equals(nameOfViewItem)) >= 0) return this;

				uiViewItemList.Add(uiViewItem);
				uiViewItem.viewItemName = nameOfViewItem;
				uiViewItem.onValueChanged = onChangeValue;
				uiViewItem.Init();
				return this;
			}
			public ViewItemCollector Add(TextInputField uiViewItem, string nameOfViewItem, Action<string> onSubmit, Action<string> onChangeValue)
			{
				if(uiViewItem == null || nameOfViewItem.IsNullOrWhiteSpace())
				{
					UnityEngine.Debug.LogError($"UIViewItem({uiViewItem}) 또는 ViewItemName({nameOfViewItem}) 은 null 또는 빈 값을 허용하지 않습니다.");
					return this;
				}
				if(uiViewItemList.Contains(uiViewItem)) return this;
				if(uiViewItemList.FindIndex(i => i.viewItemName.Equals(nameOfViewItem)) >= 0) return this;

				uiViewItemList.Add(uiViewItem);
				uiViewItem.viewItemName = nameOfViewItem;
				uiViewItem.onSubmit = onSubmit;
				uiViewItem.onValueChanged = onChangeValue;
				uiViewItem.Init();
				return this;
			}
			public bool TryGetViewItem<T>(string nameOfViewItem, out T viewItem) where T : UIViewItem<T>
			{
				viewItem = null;
				if(nameOfViewItem.IsNotNullOrWhiteSpace())
				{
					int length = uiViewItemList.Count;
					for(int i = 0 ; i < length ; i++)
					{
						UIViewItem item = uiViewItemList[i];
						if(item.viewItemName.Equals(nameOfViewItem) && item is T tItem)
						{
							viewItem = tItem;
							break;
						}
					}
				}
				return viewItem != null;
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
			ThisUIShowAndHide = gameObject.GetComponentInChildren<IUIShowAndHide>();
			if(ThisUIShowAndHide == null)
			{
				Debug.LogException(new Exception("IUIShowAndHide 컴포넌트가 없습니다. UIViewComponent 에는 최소한 1개의 UIShowAndHide 가 있어야 합니다."));
			}
		}
		sealed protected override void BaseAwake()
		{
			ThisUIShowAndHide = gameObject.GetComponentInChildren<IUIShowAndHide>();
			viewItemCollector = AwakeUIView(ViewItemCollector.New());
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
			if(viewItemCollector != null)
			{
				viewItemCollector.Dispose();
				viewItemCollector = null;
			}
		}

		protected virtual ViewItemCollector AwakeUIView(ViewItemCollector viewItemBuilder) { return viewItemBuilder; }
		protected virtual void DestroyUIView() { }
		protected virtual async Awaitable OnShowUIView() { await ThisUIShowAndHide.OnShow(); }
		protected virtual async Awaitable OnHideUIView() { await ThisUIShowAndHide.OnShow(); }

		public bool TryGetViewItem<T>(string nameOfViewItem, out T viewItem) where T : UIViewItem<T>
		{
			if(viewItemCollector != null) viewItemCollector.TryGetViewItem(nameOfViewItem, out viewItem);
			else viewItem = null;

			return viewItem != null;
		}
	}
}
