using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.SceneManagement;
namespace TF.System
{
	public abstract class SceneState : ObjectBehaviour
	{
		[ShowInInspector, DisplayAsString, EnableGUI, PropertySpace]
		public abstract string TargetSceneName { get; }
		public Scene TargetScene { get; private set; }
		[ShowInInspector, ReadOnly]
		public SystemState[] SystemStateInTargetScene { get; private set; }

		public enum SceneStateType
		{
			Close,
			Enable,
			Open,
			Disable,
		}
		public SceneStateType CurrentSceneState { get; set; }

		protected override void BaseAwake()
		{
			CurrentSceneState = SceneStateType.Close;
			TargetScene = SceneManager.GetSceneByName(TargetSceneName);
			enabled = false;
		}

		protected override void BaseEnable()
		{
			if(CurrentSceneState == SceneStateType.Close)
			{
				OpenScene();
			}
		}
		protected override void BaseDisable()
		{
			if(CurrentSceneState == SceneStateType.Open)
			{
				CloseScene();
			}
		}
		protected override void BaseDestroy()
		{
			base.BaseDestroy();
		}

		protected virtual async void OpenScene()
		{
			CurrentSceneState = SceneStateType.Enable;
			await SceneManager.LoadSceneAsync(TargetSceneName, LoadSceneMode.Additive);
			TargetScene = SceneManager.GetSceneByName(TargetSceneName);
			SystemStateInTargetScene = GetSystemStateInTargetScene();
			AttachSceneState();
			await DelayOpenScene();
			CurrentSceneState = SceneStateType.Open;
		}
		protected virtual async void CloseScene()
		{
			CurrentSceneState = SceneStateType.Disable;
			DetachSceneState();
			await DelayCloseScene();
			SystemStateInTargetScene = null;
			await SceneManager.UnloadSceneAsync(TargetSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
			CurrentSceneState = SceneStateType.Close;
		}

		private SystemState[] GetSystemStateInTargetScene()
		{
			if(!TargetScene.IsValid() || !TargetScene.isLoaded) return new SystemState[0];

			return TargetScene.GetRootGameObjects()
				.SelectMany(item => item.GetComponentsInChildren<SystemState>(true))
				.ToArray();
		}
		private void AttachSceneState()
		{
			if(!TargetScene.IsValid() || !TargetScene.isLoaded) return;
			if(SystemStateInTargetScene == null) return;
			foreach(var systemState in SystemStateInTargetScene)
			{
				systemState.AttachSceneState(this);
			}
		}
		private void DetachSceneState()
		{
			if(!TargetScene.IsValid() || !TargetScene.isLoaded) return;
			if(SystemStateInTargetScene == null) return;
			foreach(var systemState in SystemStateInTargetScene)
			{
				systemState.DetachSceneState();
			}
		}
		protected virtual async Awaitable DelayOpenScene()
		{
			int length = SystemStateInTargetScene.Length;
			for(int i = 0 ; i < length ; i++)
			{
				SystemState systemState = SystemStateInTargetScene[i];
				while(systemState != null && !systemState.SystemIsReady)
				{
					await Awaitable.NextFrameAsync();
				}
			}
		}
		protected virtual async Awaitable DelayCloseScene()
		{
			int length = SystemStateInTargetScene.Length;
			for(int i = 0 ; i < length ; i++)
			{
				SystemState systemState = SystemStateInTargetScene[i];
				while(systemState != null && systemState.SystemIsReady)
				{
					await Awaitable.NextFrameAsync();
				}
			}
		}
	}
}