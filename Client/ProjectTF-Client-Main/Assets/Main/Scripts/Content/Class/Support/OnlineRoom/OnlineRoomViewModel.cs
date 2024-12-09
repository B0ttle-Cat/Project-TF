using System.Collections.Generic;
using System.Linq;

using Sirenix.OdinInspector;

using TFSystem;
using TFSystem.UI;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace TFContent
{
	public class OnlineRoomViewModel : UIViewModelComponent, IOnlineRoomUserListUpdate
	{
		[SerializeField] TMP_Text userName;
		[SerializeField] Button backMainMenu;
		[SerializeField] Button backOnlineLobby;
		[SerializeField] Button startGamePlay;
		[SerializeField, ReadOnly]
		private bool onClick;



		private List<(int userIdx, string nickname)> userList;

		protected override void AwakeUIView(ref ViewItemSetter viewItemSetter)
		{
			backMainMenu.onClick.AddListener(async () => await WaitOnClick(OnBackMainMenu()));
			backOnlineLobby.onClick.AddListener(async () => await WaitOnClick(OnBackOnlineLobby()));
			startGamePlay.onClick.AddListener(async () => await WaitOnClick(OnStartGamePlay()));

			async Awaitable WaitOnClick(Awaitable awaitable)
			{
				if(onClick) return;
				onClick = true;
				await awaitable;
				onClick = false;
			}
		}

		protected override void DestroyUIView()
		{
			base.DestroyUIView();

			if(userList != null) userList.Clear();
			userList = null;
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

		void IOnlineRoomUserListUpdate.OnUserListUpdate(List<(int userIdx, string nickname)> userList)
		{
			if(userName == null) return;

			this.userList = userList;
			userName.text = string.Join('\n', this.userList.Select(i => $"{i.nickname} ({i.userIdx})"));
		}

		void IOnlineRoomUserListUpdate.OnEnterUser(int userIdx, string nickname)
		{
			if(userName == null) return;

			userList.Add((userIdx, nickname));
			userName.text += $"\n{nickname} ({userIdx})";
		}

		void IOnlineRoomUserListUpdate.OnLeaveUser(int userIdx)
		{
			if(userName == null) return;

			int findIndex = userList.FindIndex(i => i.userIdx == userIdx);
			if(findIndex >= 0)
			{
				userList.RemoveAt(findIndex);
				userName.text = string.Join('\n', this.userList.Select(i => $"{i.nickname} ({i.userIdx})"));
			}
		}
	}
}
