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
				var iShowAndHide = mainButtonView.GetComponent<IUIShowAndHide>();
				iShowAndHide.ThisUIShowAndHide = mainButtonView.GetComponent<UIShowAndHide>();
				iShowAndHide.InitShow();
			}
			if(ThisContainer.TryGetComponent<CreateGameView>(out createGameView))
			{
				var iShowAndHide = createGameView.GetComponent<IUIShowAndHide>();
				iShowAndHide.ThisUIShowAndHide = createGameView.GetComponent<UIShowAndHide>();
				iShowAndHide.InitHide();
				createGameView.gameObject.SetActive(false);
			}
			return false;
		}
	}
}
