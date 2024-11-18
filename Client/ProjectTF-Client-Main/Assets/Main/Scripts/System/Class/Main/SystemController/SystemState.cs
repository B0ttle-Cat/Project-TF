using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TF.System
{
	public abstract class SystemState : ObjectBehaviour
	{
		[ShowInInspector, DisplayAsString, EnableGUI, PropertySpace(0, 10)]
		public bool SystemIsReady { get; private set; }
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
			SystemIsReady = AwakeOnSystem();
		}
		sealed protected override void BaseDestroy()
		{
			DestroyOnSystems();
			AppController = null;
			SystemIsReady = false;
		}

		public virtual bool AwakeOnSystem() { return false; }
		public virtual void DestroyOnSystems() { }

		internal void AttachSceneState(SceneState sceneState)
		{
			Async(sceneState);
			async void Async(SceneState sceneState)
			{
				SceneState = sceneState;
				if(!SystemIsReady)
				{
					await StartWaitSystem();
					SystemIsReady = true;
				}
			}
		}
		internal void DetachSceneState()
		{
			Async();
			async void Async()
			{
				if(SceneState == null) return;
				if(SystemIsReady)
				{
					await EndedWaitSystem();
					SystemIsReady = false;
				}
				SceneState = null;
			}
		}

		public virtual async Awaitable StartWaitSystem() { return; }
		public virtual async Awaitable EndedWaitSystem() { return; }
	}
}
