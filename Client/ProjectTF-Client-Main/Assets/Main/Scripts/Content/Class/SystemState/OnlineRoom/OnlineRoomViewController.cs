using TF.System;
using TF.System.UI;

using UnityEngine;

namespace TF.Content
{
	public enum OnlineRoomViewState
	{
		None,

		NextSceneState_MainMenuState = 100,
		NextSceneState_OnlineLobbyState,
		NextSceneState_GamePlayState,
	}

	public class OnlineRoomViewController : UIViewController<OnlineRoomViewState>
	{
		private MainMenuSystem mainMenuSystem;
		private IApplication AppController => mainMenuSystem == null ? null : mainMenuSystem.AppController;
		private ISceneController SceneController => AppController?.SceneController;

		protected override void AwakeInController()
		{
			if(ThisContainer.TryGetObject<MainMenuSystem>(out var systemObject))
			{
				mainMenuSystem = systemObject;
			}
		}

		protected override void DestroyInController()
		{
			mainMenuSystem = null;
		}

		protected override void StartInController()
		{
		}

		protected override void InitViewState(OnlineRoomViewState viewState)
		{
			CheckChangeScene(ref viewState);

			base.InitViewState(viewState);
		}
		protected override async Awaitable ChangeViewState(OnlineRoomViewState viewState)
		{
			CheckChangeScene(ref viewState);

			await base.ChangeViewState(viewState);
		}

		private void CheckChangeScene(ref OnlineRoomViewState viewState)
		{
			if(viewState == OnlineRoomViewState.NextSceneState_MainMenuState)
			{
				SceneController?.ChangeSceneState(ISceneController.SceneState.MainMenuState, null);
				viewState = OnlineRoomViewState.None;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_OnlineLobbyState)
			{
				SceneController?.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState, null);
				viewState = OnlineRoomViewState.None;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_GamePlayState)
			{
				SceneController?.ChangeSceneState(ISceneController.SceneState.GamePlayState, null);
				viewState = OnlineRoomViewState.None;
			}
		}
	}
}
