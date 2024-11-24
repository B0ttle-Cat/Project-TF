using System;
using System.Collections.Generic;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace TF.System.UI
{
	public abstract class UIViewController<TViewState> : ObjectBehaviour, IUIViewController<TViewState> where TViewState : Enum
	{
		[SerializeField, EnumPaging]
		private TViewState initViewState;
#if UNITY_EDITOR
		protected override void BaseValidate()
		{
			IUIViewController<TViewState> uiViewController = this;
			uiViewController.OnInitViewState(initViewState);
		}
#endif
		[ShowInInspector, ReadOnly]
		protected TViewState CurrentViewState { get; private set; }
		[ShowInInspector, ReadOnly]
		protected SystemState ThisSystemState { get; private set; }
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

		[ShowInInspector, ReadOnly]
		private bool isViewUpdate { get; set; }

		private Awaitable awaitable { get; set; }

		protected override void BaseAwake()
		{
			isViewUpdate = false;
			if(ThisContainer.TryGetParentObject<SystemState>(out var systemObject))
			{
				ThisSystemState = systemObject;
			}
			//var ass = await Resources.LoadAsync<GameCursor>("");

			AwakeInController();
		}
		protected abstract void AwakeInController();
		protected override void BaseDestroy()
		{
			DestroyInController();
			ThisSystemState = null;
		}
		protected abstract void DestroyInController();
		protected override void BaseStart()
		{
			StartInController();
		}
		protected abstract void StartInController();


		void IUIViewController<TViewState>.OnInitViewState(TViewState viewState)
		{
			InitViewState(viewState);
		}
		async Awaitable IUIViewController<TViewState>.OnChangeViewState(TViewState viewState)
		{
			if(awaitable != null)
			{
				awaitable.Cancel();
				awaitable = null;
			}
			awaitable = ChangeViewState(viewState);
			await awaitable;
			awaitable = null;
		}
		async void IUIViewController<TViewState>.OnChangeViewState(TViewState viewState, Action<TViewState> callback)
		{
			IUIViewController<TViewState> uiViewController = this;
			await uiViewController.OnChangeViewState(viewState);
			callback?.Invoke(CurrentViewState);
		}

		protected void InitViewState(TViewState viewState)
		{
			if(!CheckChangeState(ref viewState)) return;

			if(CurrentViewState.Equals(viewState)) return;
			isViewUpdate = true;
			try
			{
				var prevIndex = viewStateList.FindIndex(i => i.state.Equals(CurrentViewState));
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
						if(ReferenceEquals(uiViewComponent, null) || ReferenceEquals(uiViewComponent.GameObject, null)) return;
						uiViewComponent.InitHide();
						uiViewComponent.GameObject.SetActive(false);
					};
				}
				int nextCount = nextStateList.Count;
				for(int i = 0 ; i < nextCount ; i++)
				{
					IUIViewModel uiViewComponent = nextStateList[i];
					onactive += () => {
						if(ReferenceEquals(uiViewComponent, null) || ReferenceEquals(uiViewComponent.GameObject, null)) return;
						uiViewComponent.GameObject.SetActive(true);
						uiViewComponent.InitShow();
					};
				}

				CurrentViewState = viewState;
				deactive?.Invoke();
				onactive?.Invoke();
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			finally
			{
				isViewUpdate = false;
			}
		}
		protected async Awaitable ChangeViewState(TViewState viewState)
		{
			if(!CheckChangeState(ref viewState)) return;

			if(CurrentViewState.Equals(viewState)) return;
			isViewUpdate = true;
			try
			{
				var prevIndex = viewStateList.FindIndex(i => i.state.Equals(CurrentViewState));
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
					deactive += () => {
						if(ReferenceEquals(uiViewComponent, null) || ReferenceEquals(uiViewComponent.GameObject, null)) return;
						uiViewComponent.GameObject.SetActive(false);
					};
					showHideAwait.Add(uiViewComponent.OnHide());
				}
				int nextCount = nextStateList.Count;
				for(int i = 0 ; i < nextCount ; i++)
				{
					IUIViewModel uiViewComponent = nextStateList[i];
					onactive += () => {
						if(ReferenceEquals(uiViewComponent, null) || ReferenceEquals(uiViewComponent.GameObject, null)) return;
						uiViewComponent.GameObject.SetActive(true);
					};
					showHideAwait.Add(uiViewComponent.OnShow());
				}

				CurrentViewState = viewState;
				onactive?.Invoke();
				await AwaitableUtility.ParallelWaitAll(showHideAwait.ToArray());
				deactive?.Invoke();
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			finally
			{
				isViewUpdate = false;
			}
		}
		protected abstract bool CheckChangeState(ref TViewState viewState);

		private void RemoveDuplicatesStatet(List<UIViewModelComponent> prevStateList, List<UIViewModelComponent> nextStateList)
		{
			HashSet<UIViewModelComponent> duplicates = new HashSet<UIViewModelComponent>(prevStateList);
			duplicates.IntersectWith(nextStateList);
			prevStateList.RemoveAll(item => duplicates.Contains(item));
			nextStateList.RemoveAll(item => duplicates.Contains(item));
		}
	}
}
