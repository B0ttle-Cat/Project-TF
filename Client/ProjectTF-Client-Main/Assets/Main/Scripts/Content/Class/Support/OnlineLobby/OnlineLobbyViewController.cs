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
