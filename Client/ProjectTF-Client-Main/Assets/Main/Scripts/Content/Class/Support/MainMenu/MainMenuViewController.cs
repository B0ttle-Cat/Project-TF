using System;

using TFSystem;
using TFSystem.UI;

using UnityEngine;

namespace TFContent
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
		private IApplicationController AppController => ThisSystemState == null ? null : ThisSystemState.AppController;
		private ISceneController SceneController => AppController?.SceneController;

		protected override void AwakeInController()
		{

		}

		protected override void DestroyInController()
		{

		}

		protected override void StartInController()
		{
		}

		protected override bool CheckChangeState(ref MainMenuViewState viewState)
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
					SceneController?.ChangeSceneState(ISceneController.SceneState.OnlineRoomState, null);
					viewState = MainMenuViewState.None;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return false;
			}
			return true;
		}
	}
}
