using System;

using BC.ODCC;

using UnityEngine;

namespace TF.System
{
	public interface ISceneController : IOdccComponent
	{
		public enum SceneName
		{
			IntroScene          = 0,
			MainMenuScene       = 1,
			OnlineLobbyScene    = 2,
			GamePlayScene       = 3,
			GamePlayUIScene     = 4,
		}
		[Flags]
		public enum SceneNameMask
		{
			Nothing = 0,
			IntroScene          = 1<<SceneName.IntroScene,
			MainMenuScene       = 1<<SceneName.MainMenuScene,
			OnlineLobbyScene    = 1<<SceneName.OnlineLobbyScene,
			GamePlayScene       = 1<<SceneName.GamePlayScene,
			GamePlayUIScene     = 1<<SceneName.GamePlayUIScene,
		}
		public enum SceneState
		{
			NoneState           = 0,
			MainMenuState       = 1,
			OnlineLobbyState    = 2,
			GamePlayState       = 3,
		}
		[Flags]
		public enum SceneStateMask
		{
			AnyState = 0,
			NoneState           = 1 << SceneState.NoneState,
			MainMenuState       = 1 << SceneState.MainMenuState,
			OnlineLobbyState    = 1 << SceneState.OnlineLobbyState,
			GamePlayState       = 1 << SceneState.GamePlayState,
		}
		public SceneState CurrentState { get; }

		public Awaitable ChangeSceneState(SceneState nextState);
		public void ChangeSceneState(SceneState state, Action<SceneState> callback);
	}
}
