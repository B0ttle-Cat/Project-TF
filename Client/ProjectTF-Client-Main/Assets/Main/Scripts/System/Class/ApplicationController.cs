using BC.ODCC;
namespace TF.System
{
	public class ApplicationController : ObjectBehaviour, IApplication
	{
		private ISceneController sceneController;
		private ISystemController systemController;
		private IResourcesController resourcesController;
		public ISceneController SceneController => sceneController;
		public ISystemController SystemController => systemController;
		public IResourcesController ResourcesController => resourcesController;
		public override void BaseAwake()
		{
			ThisContainer.TryGetComponent(out sceneController);
			ThisContainer.TryGetComponent(out systemController);
			ThisContainer.TryGetComponent(out resourcesController);
		}

	}
}