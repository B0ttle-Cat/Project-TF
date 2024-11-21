using Sirenix.OdinInspector;

using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class MainMenuSystem : SystemState
	{
		[SerializeField, EnumPaging]
		private MainMenuViewState initViewState;
		private IUIViewController<MainMenuViewState> viewController;

		protected override void BaseValidate()
		{
			if(gameObject.TryGetComponent(out IUIViewController<MainMenuViewState> _viewController))
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
