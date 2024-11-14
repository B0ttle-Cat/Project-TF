using BC.ODCC;
namespace TF.System
{
	public class ApplicationController : ObjectBehaviour
		, IApplication
	{
		ISceneController sceneController;
		ISystemController systemController;

		public ISceneController SceneController => sceneController;
		public ISystemController SystemController => systemController;

		public override void BaseAwake()
		{
			ThisContainer.TryGetComponent<ISceneController>(out sceneController);
			ThisContainer.TryGetComponent<ISystemController>(out systemController);
		}
	}
}