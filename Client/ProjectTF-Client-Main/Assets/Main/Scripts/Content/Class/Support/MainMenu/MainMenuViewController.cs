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

		protected override async Awaitable<MainMenuViewState> CheckChangeState(MainMenuViewState viewState)
		{
			try
			{
				if(viewState == MainMenuViewState.NextSceneState_OnlineLobbyState)
				{
					bool change = await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState);
					viewState = change ? MainMenuViewState.None : MainMenuViewState.MainView;
				}
				else if(viewState == MainMenuViewState.NextSceneState_OnlineRoomState)
				{
					bool change = await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.OnlineRoomState);
					viewState = change ? MainMenuViewState.None : MainMenuViewState.MainView;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return MainMenuViewState.None;
			}
			return viewState;
		}
	}
}
