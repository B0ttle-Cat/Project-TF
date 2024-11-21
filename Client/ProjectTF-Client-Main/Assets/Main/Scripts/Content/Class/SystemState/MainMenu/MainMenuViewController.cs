using System;

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
		NextSceneState_OnlineRoomState,
	}
	public class MainMenuViewController : UIViewController<MainMenuViewState>
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
			try
			{
				if(viewState == MainMenuViewState.NextSceneState_OnlineLobbyState)
				{
					SceneController?.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState, null);
					viewState = MainMenuViewState.None;
				}
				else if(viewState == MainMenuViewState.NextSceneState_OnlineRoomState)
				{
					SceneController?.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState, null);
					viewState = MainMenuViewState.None;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}
}
