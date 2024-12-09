using TFSystem;
using TFSystem.UI;

using UnityEngine;

namespace TFContent
{
	public enum OnlineRoomViewState
	{
		None,
		OnlineRoomsDefaultState,

		NextSceneState_MainMenuState = 100,
		NextSceneState_OnlineLobbyState,
		NextSceneState_GamePlayState,
	}

	public class OnlineRoomViewController : UIViewController<OnlineRoomViewState>
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

		protected override async Awaitable<OnlineRoomViewState> CheckChangeState(OnlineRoomViewState viewState)
		{
			if(viewState == OnlineRoomViewState.NextSceneState_MainMenuState)
			{
				await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.MainMenuState);
				viewState = OnlineRoomViewState.None;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_OnlineLobbyState)
			{
				await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState);
				viewState = OnlineRoomViewState.None;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_GamePlayState)
			{
				await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.GamePlayState);
				viewState = OnlineRoomViewState.None;
			}
			return viewState;
		}
	}
}
