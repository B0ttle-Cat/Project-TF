using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.SceneManagement;

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
#if UNITY_EDITOR
				// 강제로 ApplicationScene 열기
				UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoad;
				UnityEngine.SceneManagement.SceneManager.LoadScene(SceneController.ApplicationScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
				void SceneLoad(Scene scene, LoadSceneMode arg1)
				{
					if(scene.name != SceneController.ApplicationScene) return;
					ApplicationController _AppController = FindAnyObjectByType<ApplicationController>();

					UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneLoad;
					_AppController.EditerOnly_AppStartState = ISceneController.SceneState.NoneState;
					AppController = _AppController;

					Debug.LogError($"\"{SceneController.ApplicationScene}\"씬을 강제로 로드하였습니다. 시작 씬이을 확인해 주세요.");
				}
#else
				Debug.LogError($"{nameof(ApplicationController)}가 없습니다. 시작 씬이 \"{SceneController.ApplicationScene}\"이 맞는지 확인하세요.");
				return;
#endif
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
