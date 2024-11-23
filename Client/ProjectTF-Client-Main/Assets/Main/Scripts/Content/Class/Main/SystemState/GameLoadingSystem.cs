using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class GameLoadingSystem : SystemState
	{
		[SerializeField]
		private IUIViewController<GameLoadingViewState> loadingUI;

		protected override void AwakeOnSystem()
		{
			ThisContainer.TryGetChildObject(out loadingUI);
		}
		protected override async Awaitable StartWaitSystem()
		{
			if(loadingUI == null) return;
			await loadingUI.OnChangeViewState(GameLoadingViewState.Loading);
		}
		protected override async Awaitable EndedWaitSystem()
		{
			if(loadingUI == null) return;
			await loadingUI.OnChangeViewState(GameLoadingViewState.None);
		}
		protected override void DestroyOnSystems()
		{
			loadingUI = null;
		}
	}
}
