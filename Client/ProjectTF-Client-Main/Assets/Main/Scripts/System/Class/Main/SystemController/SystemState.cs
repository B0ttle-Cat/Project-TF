using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TF.System
{
	public abstract class SystemState : ObjectBehaviour
	{
		[ShowInInspector, DisplayAsString, EnableGUI, PropertyOrder(-4), PropertySpace(0, 10)]
		public bool SystemIsReady { get; private set; } = false;
		public SceneState SceneState { get; private set; }
		public IApplication AppController { get; private set; }
		sealed protected override void BaseAwake()
		{
			AppController = FindAnyObjectByType<ApplicationController>();
			if(AppController == null)
			{
				Debug.LogError($"{nameof(ApplicationController)} 없습니다. 시작 씬이 \"{SceneController.ApplicationScene}\"이 맞는지 확인하세요.");
				return;
			}
			SystemIsReady = false;
			AwakeOnSystem();
		}
		sealed protected override void BaseDestroy()
		{
			DestroyOnSystems();
			AppController = null;
			SystemIsReady = false;
		}

		protected abstract void AwakeOnSystem();
		protected abstract void DestroyOnSystems();

		internal void AttachSceneState(SceneState sceneState)
		{
			Async(sceneState);
			async void Async(SceneState sceneState)
			{
				SceneState = sceneState;
				await StartWaitSystem();
				SystemIsReady = true;
			}
		}
		internal void DetachSceneState()
		{
			Async();
			async void Async()
			{
				if(SceneState == null) return;
				await EndedWaitSystem();
				SystemIsReady = false;
				SceneState = null;
			}
		}

		protected abstract Awaitable StartWaitSystem();// { return; }
		protected abstract Awaitable EndedWaitSystem();// { return; }
	}
}
