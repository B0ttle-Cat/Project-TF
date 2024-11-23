using Sirenix.OdinInspector;

using TF.System;
using TF.System.UI;

using UnityEngine;
using UnityEngine.UI;

namespace TF.Content
{
	public class OnlineLobbyViewModel : UIViewModelComponent
	{
		[SerializeField] Button backMainMenu;
		[SerializeField] Button joinOnlineRoom;
		[SerializeField, ReadOnly]
		private bool onClick;


		protected override void AwakeUIView(ref ViewItemSetter viewItemSetter)
		{
			backMainMenu.onClick.AddListener(async () => await WaitOnClick(OnBackMainMenu()));
			joinOnlineRoom.onClick.AddListener(async () => await WaitOnClick(OnJoinOnlineRoom()));

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
			if(ThisContainer.TryGetObject<IUIViewController<OnlineLobbyViewState>>(out var view))
			{
				await view.OnChangeViewState(OnlineLobbyViewState.NextSceneState_MainMenuState);
			}
		}

		private async Awaitable OnJoinOnlineRoom()
		{
			if(ThisContainer.TryGetObject<IUIViewController<OnlineLobbyViewState>>(out var view))
			{
				await view.OnChangeViewState(OnlineLobbyViewState.NextSceneState_OnlineRoomState);
			}
		}
	}
}
