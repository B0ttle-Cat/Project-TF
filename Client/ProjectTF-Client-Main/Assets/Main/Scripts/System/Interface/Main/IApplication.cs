using BC.ODCC;

namespace TF.System
{
	public interface IApplication : IOdccObject
	{
		ISceneController SceneController { get; }
		ISystemController SystemController { get; }
		IResourcesController ResourcesController { get; }
	}
}
