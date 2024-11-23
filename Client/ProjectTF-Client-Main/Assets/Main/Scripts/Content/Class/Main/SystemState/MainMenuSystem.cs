using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class MainMenuSystem : SystemState
	{
		private IUIViewController<MainMenuViewState> viewController;

		protected override void AwakeOnSystem()
		{
			ThisContainer.TryGetChildObject(out viewController);
		}
		protected override void DestroyOnSystems()
		{

		}

		protected override async Awaitable StartWaitSystem()
		{
			await viewController.OnChangeViewState(MainMenuViewState.MainView);
		}
		protected override async Awaitable EndedWaitSystem()
		{
			await viewController.OnChangeViewState(MainMenuViewState.None);
		}

	}
}
