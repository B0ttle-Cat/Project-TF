using TFSystem;
using TFSystem.UI;

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

		protected override bool CheckChangeState(ref OnlineRoomViewState viewState)
		{
			if(viewState == OnlineRoomViewState.NextSceneState_MainMenuState)
			{
				ThisSystemState?.ChangeSceneState(ISceneController.SceneState.MainMenuState);
				viewState = OnlineRoomViewState.None;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_OnlineLobbyState)
			{
				ThisSystemState?.ChangeSceneState(ISceneController.SceneState.OnlineLobbyState);
				viewState = OnlineRoomViewState.None;
			}
			else if(viewState == OnlineRoomViewState.NextSceneState_GamePlayState)
			{
				ThisSystemState?.ChangeSceneState(ISceneController.SceneState.GamePlayState);
				viewState = OnlineRoomViewState.None;
			}
			return true;
		}
	}
}
