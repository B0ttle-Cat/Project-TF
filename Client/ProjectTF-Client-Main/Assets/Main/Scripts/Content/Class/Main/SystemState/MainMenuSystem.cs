using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class MainMenuSystem : SystemState
	{
		private IUIViewController<MainMenuViewState> viewController;

		public override void AwakeOnSystem()
		{
			ThisContainer.TryGetChildObject(out viewController);
		}

		public override async Awaitable StartWaitSystem()
		{
			await viewController.OnChangeViewState(MainMenuViewState.MainView);
		}
	}
}
