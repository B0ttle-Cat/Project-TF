using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class OnlineLobbySystem : SystemState
	{
		IUIViewController<OnlineLobbyViewState> viewController;
		protected override void AwakeOnSystem()
		{
			ThisContainer.TryGetChildObject(out viewController);
		}
		protected override void DestroyOnSystems()
		{
			viewController = null;
		}
		async protected override Awaitable StartWaitSystem()
		{
			await viewController.OnChangeViewState(OnlineLobbyViewState.OnlineLobbyDefaultState);
		}

		async protected override Awaitable EndedWaitSystem()
		{
			await viewController.OnChangeViewState(OnlineLobbyViewState.None);
		}


	}
}
