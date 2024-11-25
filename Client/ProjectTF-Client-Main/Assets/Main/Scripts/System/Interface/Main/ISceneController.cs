using System;
using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;

namespace TFSystem
{
	public interface ISceneController : IOdccComponent
	{
		public enum SceneName
		{
			IntroScene          = 0,
			MainMenuScene       = 1,

			OnlineLobbyScene    = 2,
			OnlineRoomScene     = 3,

			GameLoadingScene    = 4,
			GamePlayScene       = 5,
			GamePlayUIScene     = 6,
		}
		[Flags]
		public enum SceneNameMask
		{
			Nothing = 0,
			IntroScene          = 1<<SceneName.IntroScene,
			MainMenuScene       = 1<<SceneName.MainMenuScene,
			OnlineLobbyScene    = 1<<SceneName.OnlineLobbyScene,
			OnlineRoomScene     = 1<<SceneName.OnlineRoomScene,
			GameLoadingScene    = 1<<SceneName.GameLoadingScene,
			GamePlayScene       = 1<<SceneName.GamePlayScene,
			GamePlayUIScene     = 1<<SceneName.GamePlayUIScene,
		}
		public enum SceneState
		{
			NoneState           = 0,
			MainMenuState       = 1,
			OnlineLobbyState    = 2,
			OnlineRoomState     = 3,
			GamePlayState       = 4,
		}
		[Flags]
		public enum SceneStateMask
		{
			AnyState = 0,
			NoneState           = 1 << SceneState.NoneState,
			MainMenuState       = 1 << SceneState.MainMenuState,
			OnlineLobbyState    = 1 << SceneState.OnlineLobbyState,
			OnlineRoomState     = 1 << SceneState.OnlineRoomState,
			GamePlayState       = 1 << SceneState.GamePlayState,
		}
		public SceneState CurrentState { get; }
		public Stack<SceneState> SceneChangeStack { get; }

		public Awaitable ChangeSceneState(SceneState nextState);
		public void ChangeSceneState(SceneState nextState, Action<SceneState> callback);

	}
}
