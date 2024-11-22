using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class GameLoadingSystem : SystemState
	{
		[SerializeField]
		private IUIViewController<GameLoadingViewState> loadingUI;

		public override void AwakeOnSystem()
		{
			ThisContainer.TryGetChildObject(out loadingUI);
		}
		public override async Awaitable StartWaitSystem()
		{
			if(loadingUI == null) return;
			await loadingUI.OnChangeViewState(GameLoadingViewState.Loading);
		}
		public override async Awaitable EndedWaitSystem()
		{
			if(loadingUI == null) return;
			await loadingUI.OnChangeViewState(GameLoadingViewState.None);
		}
		public override void DestroyOnSystems()
		{
			loadingUI = null;
		}
	}
}
