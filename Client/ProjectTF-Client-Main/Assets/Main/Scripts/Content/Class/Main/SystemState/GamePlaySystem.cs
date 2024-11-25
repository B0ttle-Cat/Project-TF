using TFSystem;

using UnityEngine;

namespace TFContent
{
	public class GamePlaySystem : SystemState
	{
		//private IUIViewController<MainMenuViewState> viewController;

		protected override void AwakeOnSystem()
		{
			//	ThisContainer.TryGetChildObject(out viewController);
		}
		protected override void DestroyOnSystems()
		{
			//	viewController = null;
		}

		protected override async Awaitable StartWaitSystem()
		{
			//	viewController.OnChangeViewState(MainMenuViewState.MainView);
		}

		protected override async Awaitable EndedWaitSystem()
		{
		}

	}
}
