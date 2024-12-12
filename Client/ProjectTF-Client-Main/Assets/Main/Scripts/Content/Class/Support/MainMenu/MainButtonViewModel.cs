using Sirenix.OdinInspector;

using TFSystem;
using TFSystem.UI;

using UnityEngine;
using UnityEngine.UI;

namespace TFContent
{
	public class MainButtonViewModel : UIViewModelComponent
	{
		[SerializeField] private Button createGameButton;
		[SerializeField] private Button joinGameButton;
		[SerializeField] private Button statisticsButton;
		[SerializeField] private Button settingButton;
		[SerializeField] private Button exitGameButton;
		[SerializeField, ReadOnly]
		private bool onClick;

		protected override void AwakeUIView(ref ViewItemSetter viewItemSetter)
		{
			onClick = false;
			createGameButton.onClick.AddListener(async () => await WaitOnClick(OnCreateGameButton()));
			joinGameButton.onClick.AddListener(async () => await WaitOnClick(OnJoinGameButton()));
			statisticsButton.onClick.AddListener(async () => await WaitOnClick(OnStatisticsButton()));
			settingButton.onClick.AddListener(async () => await WaitOnClick(OnSettingButton()));
			exitGameButton.onClick.AddListener(async () => await WaitOnClick(OnExitGameButton()));

			async Awaitable WaitOnClick(Awaitable awaitable)
			{
				if(onClick) return;
				onClick = true;
				await awaitable;
				onClick = false;
			}
		}
		private async Awaitable OnCreateGameButton()
		{
			if(ThisContainer.TryGetObject<IUIViewController<MainMenuViewState>>(out var view))
			{
				await view.OnChangeViewState(MainMenuViewState.CreateView);
			}
		}

		private async Awaitable OnJoinGameButton()
		{
			if(ThisContainer.TryGetObject<IUIViewController<MainMenuViewState>>(out var view))
			{
				await view.OnChangeViewState(MainMenuViewState.NextSceneState_OnlineLobbyState);
			}
		}

		private async Awaitable OnStatisticsButton()
		{
			if(ThisContainer.TryGetObject<IUIViewController<MainMenuViewState>>(out var view))
			{
				await view.OnChangeViewState(MainMenuViewState.MainView);
			}
		}

		private async Awaitable OnSettingButton()
		{
			if(ThisContainer.TryGetObject<IUIViewController<MainMenuViewState>>(out var view))
			{
				await view.OnChangeViewState(MainMenuViewState.MainView);
			}
		}

		private async Awaitable OnExitGameButton()
		{
			if(ThisContainer.TryGetObject<IUIViewController<MainMenuViewState>>(out var view))
			{
				await view.OnChangeViewState(MainMenuViewState.MainView);
			}
		}
	}
}
