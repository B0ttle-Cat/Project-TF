using BC.ODCC;

using UnityEngine;
namespace TF.System
{
	public class ApplicationController : ObjectBehaviour, IApplication
	{
		private SceneController sceneController;
		private SystemController systemController;
		private ResourcesController resourcesController;
		public ISceneController SceneController => sceneController;
		public ISystemController SystemController => systemController;
		public IResourcesController ResourcesController => resourcesController;

		[SerializeField, Space]
		private ISceneController.SceneState AppStartState = ISceneController.SceneState.MainMenuState;

		public override void BaseAwake()
		{
			ThisContainer.TryGetComponent(out sceneController);
			ThisContainer.TryGetComponent(out systemController);
			ThisContainer.TryGetComponent(out resourcesController);
		}
		public override void BaseStart()
		{
			SceneController.ChangeSceneState(AppStartState, null);
		}
	}
}