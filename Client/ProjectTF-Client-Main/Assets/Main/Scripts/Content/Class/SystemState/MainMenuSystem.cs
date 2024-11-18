using TF.System;

namespace TF.Content
{
	public class MainMenuSystem : SystemState
	{
		private MainButtonView mainButtonView;
		private CreateGameView createGameView;

		public override bool AwakeOnSystem()
		{
			if(ThisContainer.TryGetComponent<MainButtonView>(out mainButtonView))
			{
				var uiView = mainButtonView.GetComponent<IUIViewComponent>();
				uiView.InitShow();
			}
			if(ThisContainer.TryGetComponent<CreateGameView>(out createGameView))
			{
				var uiView = createGameView.GetComponent<IUIViewComponent>();
				uiView.InitShow();
				createGameView.gameObject.SetActive(false);
			}
			return false;
		}
	}
}
