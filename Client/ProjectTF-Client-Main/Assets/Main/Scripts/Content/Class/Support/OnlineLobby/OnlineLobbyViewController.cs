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
				await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.MainMenuState);
				viewState = OnlineLobbyViewState.None;
			}
			else if(viewState == OnlineLobbyViewState.NextSceneState_OnlineRoomState)
			{
				await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.OnlineRoomState);
				viewState = OnlineLobbyViewState.None;
			}
			return viewState;
		}
	}
}
