using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

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
		OdccQueryCollector collectorNetworkUser;

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


			AppController.DataCarrier.AddData(nameof(WorldMapCreateDataInfo),
				new WorldMapCreateDataInfo() {
					mapSeed = Random.Range(int.MinValue, int.MaxValue),
					mapSizeXZ = new Vector2Int(8, 8),
					multipathRate = 0.4f
				});
			AppController.DataCarrier.AddData(nameof(RoomContentCreateData),
				new RoomContentCreateData() {
					roomThemeName = "DefaultTheme",
					contentPoint = new List<RoomContentCreateData.ContentPoint>() {
						new() { contentType = RoomContentType.일반방, point = 100, minCount = 0},
						new() { contentType = RoomContentType.보물창고, point = 00, minCount = 1},
						new() { contentType = RoomContentType.일반창고, point = 50, minCount = 0},
						new() { contentType = RoomContentType.무기창고, point = 50, minCount = 0},
					}
				});

			userList = new List<(int userIdx, string nickname)>();
			var queryNetworkUser = QuerySystemBuilder.CreateQuery().WithAll<NetworkUser, UserBaseData>().Build();
			collectorNetworkUser = OdccQueryCollector.CreateQueryCollector(queryNetworkUser)
				.CreateChangeListEvent(ChangeOnlineUser)
				.GetCollector();

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

			if(collectorNetworkUser != null)
			{
				collectorNetworkUser.DeleteChangeListEvent(ChangeOnlineUser);
				collectorNetworkUser = null;
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


		private void ChangeOnlineUser(ObjectBehaviour target, bool isadd)
		{
			if(target.ThisContainer.TryGetData<UserBaseData>(out var userBaseData))
			{
				var userIdx = userBaseData.UserIdx;
				var nickname = userBaseData.Nickname;
				int findIndex = userList.FindIndex(i => i.userIdx == userIdx);
				if(isadd)
				{
					if(findIndex < 0)
					{
						userList.Add((userBaseData.UserIdx, userBaseData.Nickname));
					}
				}
				else
				{
					if(findIndex >= 0)
					{
						userList.RemoveAt(findIndex);
					}
				}
			}
			if(userList.Count > 0)
			{
				userName.text = $"Enter User List ({userList.Count}):\n";
				userName.text += string.Join('\n', this.userList.Select(i => $"{i.nickname} ({i.userIdx})"));
			}
			else
			{
				userName.text = "Empty User";
			}
		}
	}
}
