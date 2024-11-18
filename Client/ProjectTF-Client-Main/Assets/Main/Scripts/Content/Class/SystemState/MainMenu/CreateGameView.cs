using Sirenix.OdinInspector;

using TF.System;
using TF.System.UI;

using UnityEngine;
using UnityEngine.UI;

namespace TF.Content
{
	public class CreateGameView : UIViewModelComponent
	{
		[SerializeField] private Button cancelButton;
		[SerializeField] private Button helpButton;
		[SerializeField] private Button confirmButton;
		[SerializeField, ReadOnly]
		private bool onClick;

		#region View Item Setting
		[SerializeField, FoldoutGroup("Room Title")]
		private TextInputField roomTitle;
		public enum RoomPublicType
		{
			Public, Private
		}
		[SerializeField, FoldoutGroup("Room Public Type")]
		private  ToggleGroupView<RoomPublicType> roomPublicType;

		public enum NumberOfPlayer
		{
			None = 0,
			Player_1 = 1,
			Player_2 = 2,
			Player_3 = 3,
			Player_4 = 4
		}
		[SerializeField, FoldoutGroup("Number Of Player")]
		private DropdownView<NumberOfPlayer> numberOfPlayer;

		#endregion

		protected override ViewItemCollector AwakeUIView(ViewItemCollector viewItemCollector)
		{
			viewItemCollector.Add(roomTitle, nameof(roomTitle), (_) => { }, (_) => { });
			viewItemCollector.Add(roomPublicType, nameof(roomPublicType), (_) => { });
			viewItemCollector.Add(numberOfPlayer, nameof(numberOfPlayer), (_) => { });

			onClick = false;
			cancelButton.onClick.AddListener(async () => await WaitOnClick(OnCancelButton()));
			helpButton.onClick.AddListener(async () => await WaitOnClick(OnHelpButton()));
			confirmButton.onClick.AddListener(async () => await WaitOnClick(OnConfirmButton()));
			async Awaitable WaitOnClick(Awaitable awaitable)
			{
				if(onClick) return;
				onClick = true;
				await awaitable;
				onClick = false;
			}
			return viewItemCollector;
		}
		protected override async Awaitable OnShowUIView()
		{
			roomTitle.ResetValue();
			roomPublicType.ResetValue();
			numberOfPlayer.ResetValue();
			await base.OnShowUIView();
		}

		private async Awaitable OnCancelButton()
		{
			if(ThisContainer.TryGetComponent<IUIViewController<MainViewState>>(out var view))
			{
				await view.OnChangeViewState(MainViewState.MainView);
			}
		}
		private async Awaitable OnHelpButton()
		{
			if(ThisContainer.TryGetComponent<IUIViewController<MainViewState>>(out var view))
			{
				await view.OnChangeViewState(MainViewState.MainView);
			}
		}
		private async Awaitable OnConfirmButton()
		{
			if(ThisContainer.TryGetComponent<IUIViewController<MainViewState>>(out var view))
			{
				await view.OnChangeViewState(MainViewState.MainView);
			}
		}
	}
}
