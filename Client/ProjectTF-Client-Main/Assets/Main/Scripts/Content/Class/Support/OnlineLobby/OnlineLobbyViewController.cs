﻿using TF.System;
using TF.System.UI;

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

		protected override bool CheckChangeState(ref OnlineLobbyView viewState)
		{
			if(viewState == OnlineLobbyView.NextSceneState_MainMenuState)
			{
				SceneController?.ChangeSceneState(ISceneController.SceneState.MainMenuState, null);
				viewState = OnlineLobbyView.None;
			}
			else if(viewState == OnlineLobbyView.NextSceneState_OnlineRoomState)
			{
				SceneController?.ChangeSceneState(ISceneController.SceneState.OnlineRoomState, null);
				viewState = OnlineLobbyView.None;
			}
			return true;
		}
	}
}