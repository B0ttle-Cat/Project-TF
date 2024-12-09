using TFSystem;

using UnityEngine;

namespace TFContent
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

		public override async Awaitable<bool> ChangeSceneState(ISceneController.SceneState mainMenuState)
		{
			if(mainMenuState == ISceneController.SceneState.OnlineRoomState)
			{
				string roomTitle = AppController.DataCarrier.GetData("roomTitle", "");
				string nickname = AppController.DataCarrier.GetData("nickname", "");
				await AppController.NetworkController.UserGroupAPI.OnEnterRoomAsync(roomTitle, nickname);

				return await base.ChangeSceneState(mainMenuState);
			}
			return false;
		}
	}
}
