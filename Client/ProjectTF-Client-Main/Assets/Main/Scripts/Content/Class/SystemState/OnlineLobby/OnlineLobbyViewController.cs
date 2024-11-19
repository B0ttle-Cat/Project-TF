using TF.System;
using TF.System.UI;

using UnityEngine;

namespace TF.Content
{
	public enum OnlineLobbyView
	{
		None,


		NextSceneState_MainMenuState,
		NextSceneState_OnlineRoomState,
	}


	public class OnlineLobbyViewController : UIViewController<OnlineLobbyView>
	{
		protected override void InitViewState(OnlineLobbyView viewState)
		{
			CheckChangeScene(ref viewState);

			base.InitViewState(viewState);
		}
		protected override async Awaitable ChangeViewState(OnlineLobbyView viewState)
		{
			CheckChangeScene(ref viewState);

			await base.ChangeViewState(viewState);
		}

		private void CheckChangeScene(ref OnlineLobbyView viewState)
		{
			if(viewState == OnlineLobbyView.NextSceneState_MainMenuState)
			{
				if(ThisContainer.TryGetObject<MainMenuSystem>(out var systemObject))
				{
					systemObject.AppController.SceneController.ChangeSceneState(ISceneController.SceneState.MainMenuState, null);
				}
				viewState = OnlineLobbyView.None;
			}
			else if(viewState == OnlineLobbyView.NextSceneState_OnlineRoomState)
			{
				if(ThisContainer.TryGetObject<MainMenuSystem>(out var systemObject))
				{
					systemObject.AppController.SceneController.ChangeSceneState(ISceneController.SceneState.OnlineRoomState, null);
				}
				viewState = OnlineLobbyView.None;
			}
		}
	}
}
