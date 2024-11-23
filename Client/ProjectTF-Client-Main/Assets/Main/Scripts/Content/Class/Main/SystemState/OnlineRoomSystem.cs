using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class OnlineRoomSystem : SystemState
	{
		IUIViewController<OnlineRoomViewState> viewController;

		protected override void AwakeOnSystem()
		{
			ThisContainer.TryGetChildObject(out viewController);
		}

		protected override void DestroyOnSystems()
		{
			viewController = null;
		}

		protected override async Awaitable StartWaitSystem()
		{
			await viewController.OnChangeViewState(OnlineRoomViewState.OnlineRoomsDefaultState);
		}

		protected override async Awaitable EndedWaitSystem()
		{
			await viewController.OnChangeViewState(OnlineRoomViewState.None);
		}
	}
}
