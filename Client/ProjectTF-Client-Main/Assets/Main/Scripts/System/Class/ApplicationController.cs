using BC.ODCC;
namespace TF.System
{
	public class ApplicationController : ObjectBehaviour
		, IApplication
	{
		public ISceneController SceneController { get; set; }
		public ISystemController SystemController { get; set; }
	}
}