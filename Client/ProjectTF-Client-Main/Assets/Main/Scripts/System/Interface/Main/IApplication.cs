using System;

using BC.ODCC;

namespace TFSystem
{
	public interface IApplicationController : IOdccObject
	{
		ISceneController SceneController { get; }
		ISystemController SystemController { get; }
		IResourcesController ResourcesController { get; }
		INetworkController NetworkController { get; }
	}
	[Obsolete("Use IApplicationController. 이름 통일성 위해 수정")]
	public interface IApplication : IApplicationController
	{

	}
}
