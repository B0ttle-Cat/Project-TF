using System;
using System.Collections.Generic;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using UnityEngine;

namespace TF.System.UI
{
	public abstract class UIViewController<TViewState> : ComponentBehaviour, IUIViewController<TViewState> where TViewState : Enum
	{
		[SerializeField, ReadOnly]
		private TViewState currentViewState;
		[SerializeField, ReadOnly]
		private bool isViewUpdate;
		[Serializable]
		private struct ViewState
		{
			[HideLabel,Space]
			public TViewState state;

			[LabelText("UI View List")]
			[ValueDropdown("UIViewObjectList", IsUniqueList = true, ExcludeExistingValuesInList = true)]
			[ListDrawerSettings(ShowFoldout = false, ShowPaging = false, DraggableItems = false)]
			[PropertySpace(0,10)]
			public List<UIViewModelComponent> viewComponent;
#if UNITY_EDITOR
			ValueDropdownList<UIViewModelComponent> UIViewObjectList()
			{
				var thisObject = UnityEditor.Selection.activeObject;
				if(thisObject == null) return new ValueDropdownList<UIViewModelComponent>();
				if(thisObject is not GameObject gameObject) return new ValueDropdownList<UIViewModelComponent>();
				if(gameObject == null || !gameObject.TryGetComponent<UIViewController<TViewState>>(out var viewController)) return new ValueDropdownList<UIViewModelComponent>();
				if(viewController == null) return new ValueDropdownList<UIViewModelComponent>();

				ValueDropdownList<UIViewModelComponent> list = new ValueDropdownList<UIViewModelComponent>();
				var objs = viewController.GetComponentsInChildren<UIViewModelComponent>(true);
				objs.ForEach(item => list.Add(item.GetType().Name, item));
				return list;
			}
#endif
		}
		[SerializeField]
		[ListDrawerSettings(ShowFoldout = false, ShowPaging = false)]
		private List<ViewState> viewStateList = new List<ViewState>();

		protected override void BaseAwake()
		{
			isViewUpdate = false;
		}
		void IUIViewController<TViewState>.OnInitViewState(TViewState viewState)
		{
			if(currentViewState.Equals(viewState)) return;
			isViewUpdate = true;

			var prevIndex = viewStateList.FindIndex(i => i.state.Equals(currentViewState));
			var nextIndex = viewStateList.FindIndex(i => i.state.Equals(viewState));
			List<UIViewModelComponent> prevStateList = prevIndex < 0 ? new List<UIViewModelComponent>() : new List<UIViewModelComponent>(viewStateList[prevIndex].viewComponent);
			List<UIViewModelComponent> nextStateList = nextIndex < 0 ? new List<UIViewModelComponent>() : new List<UIViewModelComponent>(viewStateList[nextIndex].viewComponent);
			RemoveDuplicatesStatet(prevStateList, nextStateList);

			Action deactive = null;
			Action onactive = null;

			int prevCount = prevStateList.Count;
			for(int i = 0 ; i < prevCount ; i++)
			{
				IUIViewModel uiViewComponent = prevStateList[i];
				deactive += () => {
					uiViewComponent.GameObject.SetActive(false);
					uiViewComponent.InitHide();
				};
			}
			int nextCount = nextStateList.Count;
			for(int i = 0 ; i < nextCount ; i++)
			{
				IUIViewModel uiViewComponent = nextStateList[i];
				onactive += () => {
					uiViewComponent.GameObject.SetActive(true);
					uiViewComponent.InitShow();
				};
			}

			currentViewState = viewState;
			deactive?.Invoke();
			onactive?.Invoke();

			isViewUpdate = false;
		}

		async Awaitable IUIViewController<TViewState>.OnChangeViewState(TViewState viewState)
		{
			if(currentViewState.Equals(viewState)) return;
			isViewUpdate = true;

			var prevIndex = viewStateList.FindIndex(i => i.state.Equals(currentViewState));
			var nextIndex = viewStateList.FindIndex(i => i.state.Equals(viewState));
			List<UIViewModelComponent> prevStateList = prevIndex < 0 ? new () : new List<UIViewModelComponent>(viewStateList[prevIndex].viewComponent);
			List<UIViewModelComponent> nextStateList = nextIndex < 0 ? new () : new List<UIViewModelComponent>(viewStateList[nextIndex].viewComponent);
			RemoveDuplicatesStatet(prevStateList, nextStateList);

			List<Awaitable> showHideAwait = new List<Awaitable>();
			Action deactive = null;
			Action onactive = null;
			int prevCount = prevStateList.Count;
			for(int i = 0 ; i < prevCount ; i++)
			{
				IUIViewModel uiViewComponent = prevStateList[i];
				deactive += () => uiViewComponent.GameObject.SetActive(false);
				showHideAwait.Add(uiViewComponent.OnHide());
			}
			int nextCount = nextStateList.Count;
			for(int i = 0 ; i < nextCount ; i++)
			{
				IUIViewModel uiViewComponent = nextStateList[i];
				onactive += () => uiViewComponent.GameObject.SetActive(true);
				showHideAwait.Add(uiViewComponent.OnShow());
			}

			currentViewState = viewState;
			onactive?.Invoke();
			await AwaitableUtility.ParallelWaitAll(showHideAwait.ToArray());
			deactive?.Invoke();
			isViewUpdate = false;
		}
		async void IUIViewController<TViewState>.OnChangeViewState(TViewState viewState, Action<TViewState> callback)
		{
			IUIViewController<TViewState> uiViewController = this;
			await uiViewController.OnChangeViewState(viewState);
			callback?.Invoke(currentViewState);
		}

		private void RemoveDuplicatesStatet(List<UIViewModelComponent> prevStateList, List<UIViewModelComponent> nextStateList)
		{
			HashSet<UIViewModelComponent> duplicates = new HashSet<UIViewModelComponent>(prevStateList);
			duplicates.IntersectWith(nextStateList);
			prevStateList.RemoveAll(item => duplicates.Contains(item));
			nextStateList.RemoveAll(item => duplicates.Contains(item));
		}
	}
}
