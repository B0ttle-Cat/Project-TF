using System;

using BC.ODCC;

namespace TF.System
{
	public interface ISceneController : IOdccComponent
	{
		public enum SceneState
		{
			None,
			IntroState,
			MainMenuState,

			OnlineLobbyState,

			GamePlayState,
		}
		public SceneState CurrentState { get; }

		public void ChangeSceneState(SceneState state, Action<SceneState> callback);
	}
}
