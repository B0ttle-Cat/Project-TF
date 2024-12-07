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
					ThisSystemState?.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState);
					viewState = MainMenuViewState.None;
				}
				else if(viewState == MainMenuViewState.NextSceneState_OnlineRoomState)
				{
					ThisSystemState?.ChangeSceneState(ISceneController.SceneState.OnlineRoomState);
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
