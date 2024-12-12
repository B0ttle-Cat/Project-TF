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
			await viewController.OnChangeViewState(OnlineRoomViewState.OnlineRoomsDefaultState);
		}

		protected override async Awaitable EndedWaitSystem()
		{
			await viewController.OnChangeViewState(OnlineRoomViewState.None);
		}

		public override async Awaitable<bool> ChangeSceneState(ISceneController.SceneState mainMenuState)
		{
			if(mainMenuState == ISceneController.SceneState.GamePlayState)
			{

			}
			else if(mainMenuState == ISceneController.SceneState.OnlineLobbyState)
			{
				await AppController.NetworkController.UserGroupAPI.OnLeaveRoomAsync();
			}
			else if(mainMenuState == ISceneController.SceneState.MainMenuState)
			{
				await AppController.NetworkController.UserGroupAPI.OnLeaveRoomAsync();
				await AppController.NetworkController.OnDisconnectAsync();
			}
			return await base.ChangeSceneState(mainMenuState);
		}
	}
}
