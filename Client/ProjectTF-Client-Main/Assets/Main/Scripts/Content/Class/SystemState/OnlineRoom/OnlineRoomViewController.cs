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
				if(ThisContainer.TryGetObject<MainMenuSystem>(out var systemObject))
				{
					systemObject.AppController.SceneController.ChangeSceneState(ISceneController.SceneState.MainMenuState, null);
				}
				viewState = OnlineRoomViewState.None;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_OnlineLobbyState)
			{
				if(ThisContainer.TryGetObject<MainMenuSystem>(out var systemObject))
				{
					systemObject.AppController.SceneController.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState, null);
				}
				viewState = OnlineRoomViewState.None;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_GamePlayState)
			{
				if(ThisContainer.TryGetObject<MainMenuSystem>(out var systemObject))
				{
					systemObject.AppController.SceneController.ChangeSceneState(ISceneController.SceneState.GamePlayState, null);
				}
				viewState = OnlineRoomViewState.None;
			}
		}
	}
}
