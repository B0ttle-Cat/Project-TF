using BC.ODCC;

using UnityEngine;
namespace TF.System
{
	public class ApplicationController : ObjectBehaviour, IApplication
	{
		public ISceneController SceneController { get; private set; }
		public ISystemController SystemController { get; private set; }
		public IResourcesController ResourcesController { get; private set; }

		[SerializeField, Space]
		private ISceneController.SceneState AppStartState = ISceneController.SceneState.MainMenuState;

		protected override void BaseAwake()
		{
			if(ThisContainer.TryGetComponent<SceneController>(out var sceneController))
			{
				SceneController = sceneController;
			}
			if(ThisContainer.TryGetComponent<SystemController>(out var systemController))
			{
				SystemController = systemController;
			}
			if(ThisContainer.TryGetComponent<ResourcesController>(out var resourcesController))
			{
				ResourcesController = resourcesController;
			}
		}
		protected override void BaseStart()
		{
			SceneController.ChangeSceneState(AppStartState, null);
		}
	}
}