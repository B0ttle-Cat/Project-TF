using TF.System.UI;

namespace TF.Content
{
	public enum GameLoadingViewState
	{
		None = 0,
		Loading = 1,
	}

	public class GameLoadingViewController : UIViewController<GameLoadingViewState>
	{
		protected override void AwakeInController()
		{
		}

		protected override void DestroyInController()
		{
		}

		protected override void StartInController()
		{
		}

		protected override bool CheckChangeState(ref GameLoadingViewState viewState)
		{
			return true;
		}
	}
}
