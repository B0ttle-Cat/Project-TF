using TFSystem;

using UnityEngine;

namespace TFContent
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
		public override async Awaitable<bool> ChangeSceneState(ISceneController.SceneState mainMenuState)
		{
			if(mainMenuState == ISceneController.SceneState.OnlineRoomState)
			{
				string roomTitle = AppController.DataCarrier.GetData("roomTitle", "");
				string nickname = AppController.DataCarrier.GetData("nickname", "");
				await AppController.NetworkController.UserGroupAPI.OnCreateRoomAsync(roomTitle, nickname);

				return await base.ChangeSceneState(mainMenuState);
			}
			return false;
		}
	}
}
