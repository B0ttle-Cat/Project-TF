using TF.System;
using TF.System.UI;

using UnityEngine;

namespace TF.Content
{
	public enum MainMenuViewState
	{
		None = 0,
		MainView = 1,
		CreateView = 2,

		NextSceneState_OnlineLobbyState = 100,
		NextSceneState_OnlineRoomState = 100,
	}
	public class MainMenuViewController : UIViewController<MainMenuViewState>
	{
		protected override void InitViewState(MainMenuViewState viewState)
		{
			CheckChangeScene(ref viewState);
			base.InitViewState(viewState);
		}
		protected override async Awaitable ChangeViewState(MainMenuViewState viewState)
		{
			CheckChangeScene(ref viewState);
			await base.ChangeViewState(viewState);
		}
		private void CheckChangeScene(ref MainMenuViewState viewState)
		{
			if(viewState == MainMenuViewState.NextSceneState_OnlineLobbyState)
			{
				if(ThisContainer.TryGetObject<MainMenuSystem>(out var systemObject))
				{
					systemObject.AppController.SceneController.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState, null);
				}
				viewState = MainMenuViewState.None;
			}
			else if(viewState == MainMenuViewState.NextSceneState_OnlineRoomState)
			{
				if(ThisContainer.TryGetObject<MainMenuSystem>(out var systemObject))
				{
					systemObject.AppController.SceneController.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState, null);
				}
				viewState = MainMenuViewState.None;
			}
		}
	}
}
