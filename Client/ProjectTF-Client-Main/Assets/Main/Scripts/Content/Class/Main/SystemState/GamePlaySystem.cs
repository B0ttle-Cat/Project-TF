using TFSystem;

using UnityEngine;

namespace TFContent
{
	public class GamePlaySystem : SystemState
	{
		//private IUIViewController<MainMenuViewState> viewController;
		INetworkAPI.GamePlayAPI gamePlayAPI;


		protected override void AwakeOnSystem()
		{
			if(ThisContainer.TryGetComponent<NetworkGamePlayControl>(out var networkGamePlayControl))
			{
				gamePlayAPI = networkGamePlayControl;
			}
		}
		protected override void DestroyOnSystems()
		{
			//	viewController = null;
		}

		protected override async Awaitable StartWaitSystem()
		{
			var localUser = AppController.NetworkController.UserGroupAPI.LocalUser;
			await gamePlayAPI.OnEnterGameAsync(localUser);
		}

		protected override async Awaitable EndedWaitSystem()
		{
			var localUser = AppController.NetworkController.UserGroupAPI.LocalUser;
			await gamePlayAPI.OnLeaveGameAsync(localUser);
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
