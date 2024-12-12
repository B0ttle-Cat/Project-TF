using TFSystem;
using TFSystem.UI;

using UnityEngine;

namespace TFContent
{
	public enum OnlineLobbyViewState
	{
		None,
		OnlineLobbyDefaultState,


		NextSceneState_MainMenuState,
		NextSceneState_OnlineRoomState,
	}


	public class OnlineLobbyViewController : UIViewController<OnlineLobbyViewState>
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
		protected override async Awaitable<OnlineLobbyViewState> CheckChangeState(OnlineLobbyViewState viewState)
		{
			if(viewState == OnlineLobbyViewState.NextSceneState_MainMenuState)
			{
				bool change = await ThisSystemState ?.ChangeSceneState(ISceneController.SceneState.MainMenuState);
				viewState = change ? OnlineLobbyViewState.None : OnlineLobbyViewState.OnlineLobbyDefaultState;
			}
			else if(viewState == OnlineLobbyViewState.NextSceneState_OnlineRoomState)
			{
				bool change = await ThisSystemState ?.ChangeSceneState(ISceneController.SceneState.OnlineRoomState);
				viewState = change ? OnlineLobbyViewState.None : OnlineLobbyViewState.OnlineLobbyDefaultState;
			}
			return viewState;
		}
	}
}
