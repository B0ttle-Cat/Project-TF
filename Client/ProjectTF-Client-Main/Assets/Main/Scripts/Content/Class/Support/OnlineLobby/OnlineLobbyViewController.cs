using TFSystem;
using TFSystem.UI;

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

		protected override bool CheckChangeState(ref OnlineLobbyViewState viewState)
		{
			if(viewState == OnlineLobbyViewState.NextSceneState_MainMenuState)
			{
				SceneController?.ChangeSceneState(ISceneController.SceneState.MainMenuState, null);
				viewState = OnlineLobbyViewState.None;
			}
			else if(viewState == OnlineLobbyViewState.NextSceneState_OnlineRoomState)
			{
				SceneController?.ChangeSceneState(ISceneController.SceneState.OnlineRoomState, null);
				viewState = OnlineLobbyViewState.None;
			}
			return true;
		}
	}
}
