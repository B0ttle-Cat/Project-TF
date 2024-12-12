using TFSystem;

using UnityEngine;

namespace TFContent
{
	public class GamePlaySystem : SystemState
	{
		//private IUIViewController<MainMenuViewState> viewController;

		protected override void AwakeOnSystem()
		{
			//	ThisContainer.TryGetChildObject(out viewController);
		}
		protected override void DestroyOnSystems()
		{
			//	viewController = null;
		}

		protected override async Awaitable StartWaitSystem()
		{
			//	viewController.OnChangeViewState(MainMenuViewState.MainView);
		}

		protected override async Awaitable EndedWaitSystem()
		{
		}
		public override async Awaitable<bool> ChangeSceneState(ISceneController.SceneState mainMenuState)
		{
			if(mainMenuState == ISceneController.SceneState.OnlineRoomState)
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
