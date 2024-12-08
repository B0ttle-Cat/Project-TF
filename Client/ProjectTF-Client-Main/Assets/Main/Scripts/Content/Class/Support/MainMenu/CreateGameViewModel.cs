using Sirenix.OdinInspector;

using TFSystem;
using TFSystem.UI;

using UnityEngine;
using UnityEngine.UI;

namespace TFContent
{
	public class CreateGameViewModel : UIViewModelComponent
	{
		[SerializeField] private Button cancelButton;
		[SerializeField] private Button helpButton;
		[SerializeField] private Button confirmButton;
		[SerializeField, ReadOnly]
		private bool onClick;

		#region View Item Setting
		[SerializeField, FoldoutGroup("Room Title")]
		private TextInputField roomTitle;
		[SerializeField, FoldoutGroup("User Name")]
		private TextInputField userName;
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

		protected override void AwakeUIView(ref ViewItemSetter viewItemSetter)
		{
			viewItemSetter.Add(roomTitle, nameof(roomTitle), ChangeTitle, null);
			viewItemSetter.Add(userName, nameof(userName), ChangeName, null);
			viewItemSetter.Add(roomPublicType, nameof(roomPublicType), ChangeRoomPublicType);
			viewItemSetter.Add(numberOfPlayer, nameof(numberOfPlayer), ChangeNumberOfPlayer);

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

			void ChangeTitle(string title)
			{
				if(ThisContainer.TryGetData<IDataCarrier>(out var data))
				{
					data.AddData("roomTitle", title);
				}
			}
			void ChangeName(string name)
			{
				if(ThisContainer.TryGetData<IDataCarrier>(out var data))
				{
					data.AddData("nickname", name);
				}
			}
			void ChangeRoomPublicType(RoomPublicType change)
			{
				if(ThisContainer.TryGetData<IDataCarrier>(out var data))
				{
					data.AddData("roomPublicType", change);
				}
			}
			void ChangeNumberOfPlayer(NumberOfPlayer change)
			{
				if(ThisContainer.TryGetData<IDataCarrier>(out var data))
				{
					data.AddData("numberOfPlayer", change);
				}
			}
		}
		protected override async Awaitable OnShowUIView()
		{
			roomTitle.SetupValue();
			roomPublicType.SetupValue();
			numberOfPlayer.SetupValue();
			await base.OnShowUIView();
		}

		private async Awaitable OnCancelButton()
		{
			if(ThisContainer.TryGetObject<IUIViewController<MainMenuViewState>>(out var view))
			{
				await view.OnChangeViewState(MainMenuViewState.MainView);
			}
		}
		private async Awaitable OnHelpButton()
		{
			if(ThisContainer.TryGetObject<IUIViewController<MainMenuViewState>>(out var view))
			{
				await view.OnChangeViewState(MainMenuViewState.MainView);
			}
		}
		private async Awaitable OnConfirmButton()
		{
			if(ThisContainer.TryGetObject<IUIViewController<MainMenuViewState>>(out var view))
			{
				await view.OnChangeViewState(MainMenuViewState.NextSceneState_OnlineRoomState);
			}
		}
	}
}
