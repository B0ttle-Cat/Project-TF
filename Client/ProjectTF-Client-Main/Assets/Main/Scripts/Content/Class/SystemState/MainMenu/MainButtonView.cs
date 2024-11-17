using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;

namespace TF.Content
{
	public class MainButtonView : ComponentBehaviour, IUIShowAndHide
	{
		public UIShowAndHide ThisUIShowAndHide { get; set; }

		[SerializeField] private Button createGameButton;
		[SerializeField] private Button joinGameButton;
		[SerializeField] private Button statisticsButton;
		[SerializeField] private Button settingButton;
		[SerializeField] private Button exitGameButton;
		[SerializeField, ReadOnly]
		private bool onClick;

		public override void BaseAwake()
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
			if(ThisContainer.TryGetComponent<CreateGameView>(out var view))
			{
				if(view.ThisContainer.TryGetComponent<IUIShowAndHide>(out var viewShowHide, i => i.GameObject == view.GameObject))
				{
					IUIShowAndHide thisShowHide = this;

					viewShowHide.GameObject.SetActive(true);
					Debug.Log("Start - ParallelWaitAll");
					await AwaitableUtility.ParallelWaitAll(thisShowHide.OnHide(), viewShowHide.OnShow());
					Debug.Log("End - ParallelWaitAll");
					thisShowHide.GameObject.SetActive(false);
				}
			}
		}

		private async Awaitable OnJoinGameButton()
		{
			if(ThisContainer.TryGetComponent<CreateGameView>(out var view))
			{
				if(view.ThisContainer.TryGetComponent<IUIShowAndHide>(out var viewShowHide, i => i.GameObject == view.GameObject))
				{
					IUIShowAndHide thisShowHide = this;

					viewShowHide.GameObject.SetActive(true);
					Debug.Log("Start - ParallelWaitAll");
					await AwaitableUtility.ParallelWaitAll(thisShowHide.OnHide(), viewShowHide.OnShow());
					Debug.Log("End - ParallelWaitAll"); thisShowHide.GameObject.SetActive(false);
				}
			}
		}

		private async Awaitable OnStatisticsButton()
		{
			if(ThisContainer.TryGetComponent<CreateGameView>(out var view))
			{
				if(view.ThisContainer.TryGetComponent<IUIShowAndHide>(out var viewShowHide, i => i.GameObject == view.GameObject))
				{
					await viewShowHide.OnShow();
				}
			}
		}

		private async Awaitable OnSettingButton()
		{
			if(ThisContainer.TryGetComponent<CreateGameView>(out var view))
			{
				if(view.ThisContainer.TryGetComponent<IUIShowAndHide>(out var viewShowHide, i => i.GameObject == view.GameObject))
				{
					await viewShowHide.OnShow();
				}
			}
		}

		private async Awaitable OnExitGameButton()
		{
			if(ThisContainer.TryGetComponent<CreateGameView>(out var view))
			{
				if(view.ThisContainer.TryGetComponent<IUIShowAndHide>(out var viewShowHide, i => i.GameObject == view.GameObject))
				{
					await viewShowHide.OnShow();
				}
			}
		}
	}
}
