using BC.ODCC;

namespace TFSystem
{
	public interface IApplication : IOdccObject
	{
		ISceneController SceneController { get; }
		ISystemController SystemController { get; }
		IResourcesController ResourcesController { get; }
	}
}
