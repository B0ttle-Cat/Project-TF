using System;
using System.Collections.Generic;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace TF.System
{
	public class SceneController : ComponentBehaviour, ISceneController
	{
#if UNITY_EDITOR
		private static SceneController Editor_This => FindAnyObjectByType<SceneController>();
#endif
		private bool sceneLoadingNow;
		internal const string ApplicationScene = "ApplicationScene";

		[SerializeField, ReadOnly]
		private ISceneController.SceneState currentState;
		public ISceneController.SceneState CurrentState { get => currentState; private set => currentState = value; }

		#region Scene Controller Struct
		[Serializable]
		private struct SceneControlObject
		{
			[HorizontalGroup,HideLabel, PropertyOrder(0)]
			[ValueDropdown("SceneNameList")]
			public string sceneName;
#if UNITY_EDITOR
			[HorizontalGroup(Width = 110), HideLabel, PropertyOrder(1), ShowInInspector, EnableGUI, DisplayAsString]
			public string display => " This Controller Is ";
#endif
			[HorizontalGroup, HideLabel, PropertyOrder(2)]
			[ValueDropdown("SceneObjectList")]
			public SceneState sceneControlObject;

			public async Awaitable Load()
			{
				if(sceneControlObject == null) return;

				sceneControlObject.enabled = true;
				while(sceneControlObject.CurrentSceneState is not SceneState.SceneStateType.Open)
				{
					await Awaitable.NextFrameAsync();
				}
			}
			public async Awaitable Unload()
			{
				if(sceneControlObject == null) return;

				sceneControlObject.enabled = false;
				while(sceneControlObject.CurrentSceneState is not SceneState.SceneStateType.Close)
				{
					await Awaitable.NextFrameAsync();
				}
			}

#if UNITY_EDITOR
			ValueDropdownList<string> SceneNameList()
			{
				// 결과를 저장할 리스트
				ValueDropdownList<string> list = new ValueDropdownList<string>();
				// 모든 enum 값 확인
				foreach(ISceneController.SceneName flag in Enum.GetValues(typeof(ISceneController.SceneName)))
				{
					string name = flag.ToString();
					list.Add(name, name);
				}
				return list;
			}
			ValueDropdownList<SceneState> SceneObjectList()
			{
				ValueDropdownList<SceneState> list = new ValueDropdownList<SceneState>();
				var objs = Editor_This.GetComponentsInChildren<SceneState>(true);
				objs.ForEach(item => list.Add(item.TargetSceneName, item));
				return list;
			}
#endif
		}
		[SerializeField, Space]
		private List<SceneControlObject> sceneObjects;

		[Serializable]
		private struct SceneStateGroup
		{
			[HorizontalGroup, HideLabel, PropertyOrder(0)]
			public ISceneController.SceneState sceneState;
#if UNITY_EDITOR
			[HorizontalGroup(Width = 115), HideLabel, PropertyOrder(1), ShowInInspector, EnableGUI, DisplayAsString]
			public string display1 => " Is Load Scene With";
#endif
			[HorizontalGroup, HideLabel, PropertyOrder(2)]
			public ISceneController.SceneNameMask sceneNames;

			public List<string> SceneNameList()
			{
				// 결과를 저장할 리스트
				List<string> result = new List<string>();

				// 모든 enum 값 확인
				foreach(ISceneController.SceneNameMask flag in Enum.GetValues(typeof(ISceneController.SceneNameMask)))
				{
					// 플래그가 설정되어 있는지 확인
					if(sceneNames.HasFlag(flag) && flag != ISceneController.SceneNameMask.Nothing) // None 제외
					{
						result.Add(flag.ToString());
					}
				}

				return result;
			}

		}
		[SerializeField,Space]
		private List<SceneStateGroup> sceneStateGroups;

		[Serializable]
		private struct StateLoadingScene
		{
#if UNITY_EDITOR
			[HorizontalGroup(Width = 35), HideLabel, PropertyOrder(0), ShowInInspector, EnableGUI, DisplayAsString]
			public string display1 => "From";
#endif
			[HorizontalGroup, HideLabel, PropertyOrder(1)]
			public ISceneController.SceneStateMask fromState;
#if UNITY_EDITOR
			[HorizontalGroup(Width = 20), HideLabel, PropertyOrder(2), ShowInInspector, EnableGUI, DisplayAsString]
			public string display2 => " To ";
#endif
			[HorizontalGroup, HideLabel, PropertyOrder(3)]
			public ISceneController.SceneStateMask toState;
#if UNITY_EDITOR
			[HorizontalGroup(Width = 20), HideLabel, PropertyOrder(4), ShowInInspector, EnableGUI, DisplayAsString]
			public string display3 => " Is ";
#endif
			[HorizontalGroup, HideLabel, PropertyOrder(5)]
			public ISceneController.SceneName loadSceneName;

			[HorizontalGroup("Time", order: 1, Width = 110), LabelText("Minimum Time"), ToggleLeft]
			public bool useMinimumShowTime;
			[HorizontalGroup("Time"), HideLabel]
			[EnableIf("@useMinimumShowTime"), UnityEngine.Range(0, 10)]
			public float minimumShowTime;
			public bool IsThis(ISceneController.SceneState _fromState, ISceneController.SceneState _toState)
			{
				ISceneController.SceneStateMask fromStateMask = (ISceneController.SceneStateMask)(1 << (int)_fromState);
				ISceneController.SceneStateMask toStateMask = (ISceneController.SceneStateMask)(1 << (int)_toState);
				bool from = fromState == ISceneController.SceneStateMask.AnyState || fromState.HasFlag(fromStateMask);
				bool to = toState == ISceneController.SceneStateMask.AnyState || toState.HasFlag(toStateMask);
				return from && to;
			}
		}
		[SerializeField, Space]
		private List<StateLoadingScene> stateLoadingScenes;
		#endregion

		public override void BaseAwake()
		{
			currentState = ISceneController.SceneState.NoneState;
		}

		async void ISceneController.ChangeSceneState(ISceneController.SceneState nextState, Action<ISceneController.SceneState> callback)
		{
			await DelayCommand();
			async Awaitable DelayCommand()
			{
				while(sceneLoadingNow)
				{
					await Awaitable.NextFrameAsync();
				}
				sceneLoadingNow = true;
			}

			if(!CheckNextSceneStateGroupIndex(out int nextGroupIndex))
			{
				callback?.Invoke(CurrentState);
				sceneLoadingNow = false;
				return;
			}
			bool CheckNextSceneStateGroupIndex(out int nextGroupIndex)
			{
				nextGroupIndex = -1;
				if(CurrentState == nextState)
				{
					return false;
				}
				nextGroupIndex = sceneStateGroups.FindIndex(f => f.sceneState == nextState);
				return nextGroupIndex >= 0;
			}

			await ChangeSceneState(sceneStateGroups[nextGroupIndex]);
			async Awaitable ChangeSceneState(SceneStateGroup loadStateGroup)
			{
				string loadingScene = "";
				double loadingEndTime = Time.timeAsDouble;
				if(TryGetLoadingScene(CurrentState, nextState, out StateLoadingScene loadingSceneInfo))
				{
					loadingScene = loadingSceneInfo.loadSceneName.ToString();
					loadingEndTime = Time.timeAsDouble + loadingSceneInfo.minimumShowTime;
					await LoadScene(loadingScene);
				}

				List<string> nextSceneNames = loadStateGroup.SceneNameList();
				List<string> prevSceneNames = GetDynamicLoadedScene(loadingScene);
				RemoveDuplicatesSceneName(prevSceneNames, nextSceneNames);

				int prevCount = prevSceneNames.Count;
				for(int i = 0 ; i < prevCount ; i++)
				{
					await UnloadScene(prevSceneNames[i]);
				}

				CurrentState = nextState;

				int nextCount = nextSceneNames.Count;
				for(int i = 0 ; i < nextCount ; i++)
				{
					await LoadScene(nextSceneNames[i]);
				}

				if(loadingScene.IsNotNullOrWhiteSpace())
				{
					while(Time.timeAsDouble < loadingEndTime)
					{
						await Awaitable.NextFrameAsync();
					}
					await UnloadScene(loadingScene);
				}
			}

			callback?.Invoke(CurrentState);
			sceneLoadingNow = false;
		}

		#region OtherFunction
		bool TryGetLoadingScene(ISceneController.SceneState fromState, ISceneController.SceneState toState, out StateLoadingScene loadingSceneInfo)
		{
			int length = stateLoadingScenes.Count;
			for(int i = 0 ; i < length ; i++)
			{
				if(stateLoadingScenes[i].IsThis(fromState, toState))
				{
					loadingSceneInfo = stateLoadingScenes[i];
					return true;
				}
			}
			loadingSceneInfo = default;
			return false;
		}
		List<string> GetDynamicLoadedScene(string currentLoadingScene)
		{
			List<string> loadedScene = new List<string>();
			int sceneCount = SceneManager.sceneCount;
			for(int i = 0 ; i < sceneCount ; i++)
			{
				Scene scene = SceneManager.GetSceneAt(i);
				string loadedSceneName = scene.name;

				if(loadedScene.Contains(loadedSceneName))
				{
					continue;
				}

				if(loadedSceneName.Equals(ApplicationScene))
				{
					continue;
				}
				else if(currentLoadingScene.IsNotNullOrWhiteSpace() && loadedSceneName.Equals(currentLoadingScene))
				{
					continue;
				}

				loadedScene.Add(loadedSceneName);
			}
			return loadedScene;
		}
		void RemoveDuplicatesSceneName(List<string> prevSceneNames, List<string> nextSceneNames)
		{
			HashSet<string> duplicates = new HashSet<string>(prevSceneNames);
			duplicates.IntersectWith(nextSceneNames);
			prevSceneNames.RemoveAll(item => duplicates.Contains(item));
			nextSceneNames.RemoveAll(item => duplicates.Contains(item));
		}
		async Awaitable UnloadScene(string sceneName)
		{
			int index = sceneObjects.FindIndex(item => item.sceneName.Equals(sceneName));
			if(index < 0) return;
			await sceneObjects[index].Unload();
		}
		async Awaitable LoadScene(string sceneName)
		{
			int index = sceneObjects.FindIndex(item => item.sceneName.Equals(sceneName));
			if(index < 0) return;
			await sceneObjects[index].Load();
		}
		#endregion
	}
}