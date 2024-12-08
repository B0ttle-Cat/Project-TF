using TFSystem;

using UnityEngine;

namespace TFContent
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
			AppController.DataCarrier
				.GetData("nickname", out string nickname, "")
				.GetData("userIdx", out int userIdx, -1);
			AppController.DataCarrier.ClearData();

			viewController.DataCarrier
				.AddData("nickname", nickname)
				.AddData("userIdx", userIdx);

			await viewController.OnChangeViewState(OnlineRoomViewState.OnlineRoomsDefaultState);
		}

		protected override async Awaitable EndedWaitSystem()
		{
			await viewController.OnChangeViewState(OnlineRoomViewState.None);
		}
	}
}
