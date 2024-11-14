using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine.SceneManagement;

namespace TF.System
{
	public class SceneController : ComponentBehaviour, ISceneController
	{
		private const string ApplicationScene = "ApplicationScene";

		[Serializable]
		public struct SceneStateGroup
		{
			[HorizontalGroup, HideLabel]
			public ISceneController.SceneState sceneState;
			[HorizontalGroup, HideLabel]
			[ValueDropdown("GetBuildScenes", IsUniqueList = true)]
			public List<string> sceneNames;

#if UNITY_EDITOR
			private IEnumerable<string> GetBuildScenes()
			{
				// ºôµå ¼¼ÆÃ¿¡ Æ÷ÇÔµÈ ¾ÀµéÀ» °¡Á®¿È
				return UnityEditor.EditorBuildSettings.scenes
					.Where(scene => scene.enabled)
					.Select(scene => Path.GetFileNameWithoutExtension(scene.path))
					.Where(scene => !scene.Equals(SceneController.ApplicationScene));
			}
#else
			private IEnumerable<string> GetBuildScenes() => new string[0];
#endif
		}

		public List<SceneStateGroup> sceneStateGroups;

		public ISceneController.SceneState CurrentState { get; set; }

		public void ChangeSceneState(ISceneController.SceneState state, Action<ISceneController.SceneState> callback)
		{
			int index = sceneStateGroups.FindIndex(f => f.sceneState == state);
			if(index < 0)
			{
				callback?.Invoke(CurrentState);
				return;
			}

			int sceneCount = SceneManager.sceneCount;
			for(int i = 0 ; i < sceneCount ; i++)
			{
				Scene scene = SceneManager.GetSceneAt(i);
			}


		}
	}
}