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
				bool change = await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.MainMenuState);
				viewState = change ? OnlineRoomViewState.None : OnlineRoomViewState.OnlineRoomsDefaultState;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_OnlineLobbyState)
			{
				bool change = await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState);
				viewState = change ? OnlineRoomViewState.None : OnlineRoomViewState.OnlineRoomsDefaultState;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_GamePlayState)
			{
				bool change = await ThisSystemState?.ChangeSceneState(ISceneController.SceneState.GamePlayState);
				viewState = change ? OnlineRoomViewState.None : OnlineRoomViewState.OnlineRoomsDefaultState;
			}
			return viewState;
		}
	}
}
