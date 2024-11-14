using System;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.SceneManagement;
namespace TF.System
{
	public abstract class SceneState : ObjectBehaviour
	{
		[ShowInInspector, DisplayAsString, EnableGUI]
		public abstract string TargetScene { get; }

		public enum SceneStateType
		{
			Close,
			Enable,
			Open,
			Disable,
		}
		public SceneStateType CurrentSceneState { get; set; }

		private Action delay;

		public override void BaseAwake()
		{
			CurrentSceneState = SceneStateType.Close;
			delay = null;
			enabled = false;
		}

		public override void BaseEnable()
		{
			if(CurrentSceneState == SceneStateType.Close)
			{
				delay = null;
				OpenScene();
			}
			else if(CurrentSceneState == SceneStateType.Disable)
			{
				delay = BaseEnable;
			}
			else
			{
				delay = null;
			}
		}
		public override void BaseDisable()
		{
			if(CurrentSceneState == SceneStateType.Open)
			{
				delay = null;
				CloseScene();
			}
			else if(CurrentSceneState == SceneStateType.Enable)
			{
				delay = BaseDisable;
			}
			else
			{
				delay = null;
			}
		}

		protected virtual async void OpenScene()
		{
			CurrentSceneState = SceneStateType.Enable;
			await SceneManager.LoadSceneAsync(TargetScene, LoadSceneMode.Additive);

			if(delay != null)
			{
				delay();
			}
			else
			{
				CurrentSceneState = SceneStateType.Open;
			}
		}
		protected virtual async void CloseScene()
		{
			CurrentSceneState = SceneStateType.Disable;
			await SceneManager.UnloadSceneAsync(TargetScene);

			if(delay != null)
			{
				delay();
			}
			else
			{
				CurrentSceneState = SceneStateType.Close;
			}
		}
	}
}