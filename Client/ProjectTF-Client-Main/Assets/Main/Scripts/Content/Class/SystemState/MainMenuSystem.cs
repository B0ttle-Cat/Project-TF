using Sirenix.OdinInspector;

using TF.System;

using UnityEngine;

namespace TF.Content
{
	public enum MainViewState
	{
		None = 0,
		MainView = 1,
		CreateView = 2
	}

	public class MainMenuSystem : SystemState
	{
		[SerializeField, EnumPaging]
		private MainViewState initViewState;
		private IUIViewController<MainViewState> viewController;

		protected override void BaseValidate()
		{
			if(gameObject.TryGetComponent(out IUIViewController<MainViewState> _viewController))
			{
				_viewController.OnInitViewState(initViewState);
			}
		}

		public override bool AwakeOnSystem()
		{
			ThisContainer.TryGetComponent(out viewController);
			return false;
		}

		public override async Awaitable StartWaitSystem()
		{
			await viewController.OnChangeViewState(initViewState);
		}
	}
}
