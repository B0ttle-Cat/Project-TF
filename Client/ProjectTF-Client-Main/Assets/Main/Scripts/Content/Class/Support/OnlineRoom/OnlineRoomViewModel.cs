using Sirenix.OdinInspector;

using TFSystem;
using TFSystem.UI;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace TFContent
{
	public class OnlineRoomViewModel : UIViewModelComponent
	{
		[SerializeField] TMP_Text userName;
		[SerializeField] Button backMainMenu;
		[SerializeField] Button backOnlineLobby;
		[SerializeField] Button startGamePlay;
		[SerializeField, ReadOnly]
		private bool onClick;


		protected override void AwakeUIView(ref ViewItemSetter viewItemSetter)
		{
			backMainMenu.onClick.AddListener(async () => await WaitOnClick(OnBackMainMenu()));
			backOnlineLobby.onClick.AddListener(async () => await WaitOnClick(OnBackOnlineLobby()));
			startGamePlay.onClick.AddListener(async () => await WaitOnClick(OnStartGamePlay()));

			if(ThisContainer.TryGetData<IDataCarrier>(out var dataCarrier))
			{
				if(userName != null)
				{
					dataCarrier
						.GetData("nickname", out string nickname, "None")
						.GetData("userIdx", out int userIdx, -1);

					userName.text = $"{nickname} {(userIdx == -1 ? "" : $"({userIdx})")}";
				}
			}

			async Awaitable WaitOnClick(Awaitable awaitable)
			{
				if(onClick) return;
				onClick = true;
				await awaitable;
				onClick = false;
			}
		}

		private async Awaitable OnBackMainMenu()
		{
			if(ThisContainer.TryGetObject<IUIViewController<OnlineRoomViewState>>(out var view))
			{
				await view.OnChangeViewState(OnlineRoomViewState.NextSceneState_MainMenuState);
			}
		}
		private async Awaitable OnBackOnlineLobby()
		{
			if(ThisContainer.TryGetObject<IUIViewController<OnlineRoomViewState>>(out var view))
			{
				await view.OnChangeViewState(OnlineRoomViewState.NextSceneState_OnlineLobbyState);
			}
		}
		private async Awaitable OnStartGamePlay()
		{
			if(ThisContainer.TryGetObject<IUIViewController<OnlineRoomViewState>>(out var view))
			{
				await view.OnChangeViewState(OnlineRoomViewState.NextSceneState_GamePlayState);
			}
		}

	}
}
