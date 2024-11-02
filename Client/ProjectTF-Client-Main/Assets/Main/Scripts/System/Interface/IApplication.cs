namespace TF.System
{
	public interface IApplication
	{
		ISceneController SceneController { get; set; }
		ISystemController SystemController { get; set; }
	}
}
