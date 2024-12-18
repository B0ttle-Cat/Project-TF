﻿using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using TFSystem.Network;

using UnityEngine;

namespace TFSystem
{
	public class NetworkUserGroup : ComponentBehaviour, INetworkAPI.UserGroupAPI
	{
		[SerializeField]
		QuerySystem queryNetworkUser;
		[SerializeField]
		OdccQueryCollector collectorNetworkUser;
		Dictionary<int, INetworkUser> connectUserList;

		public int LocalUserIndex { get; set; }
		public INetworkUser LocalUser { get; private set; }
		public bool IsEnter => (LocalUserIndex >=0 && connectUserList != null && connectUserList.ContainsKey(LocalUserIndex)) ? true : false;
		public INetworkController NetworkController { get; set; }

		PacketNotifyItem userEnterNty;
		PacketNotifyItem userLeaveNty;
		protected override void BaseAwake()
		{
			NetworkController = ThisContainer.GetComponent<INetworkController>();
			connectUserList = new Dictionary<int, INetworkUser>();

			userEnterNty = PacketAsyncItem.CreateNotifyItem<S2C_TEMP_CHATROOM_ENTER_NTY>(UserEnterNty, true);
			userLeaveNty = PacketAsyncItem.CreateNotifyItem<S2C_TEMP_CHATROOM_LEAVE_NTY>(UserLeaveNty, true);

			queryNetworkUser = QuerySystemBuilder.CreateQuery()
				.WithAll<NetworkUser, UserBaseData>().Build();
			collectorNetworkUser = OdccQueryCollector.CreateQueryCollector(queryNetworkUser).GetCollector();
		}
		protected override void BaseDestroy()
		{
			userEnterNty?.Dispose();
			userLeaveNty?.Dispose();
			connectUserList?.Clear();

			LocalUserIndex = -1;

			userEnterNty  = null;
			userLeaveNty  = null;
			connectUserList = null;

			NetworkController = null;

			if(queryNetworkUser != null)
			{
				OdccQueryCollector.DeleteQueryCollector(queryNetworkUser);
				queryNetworkUser = null;
				collectorNetworkUser = null;
			}
		}


		async Awaitable<INetworkAPI.UserGroupAPI.EnterRoom> OnCreateRoomAsync(string roomTitle, string nickName)
		{
			INetworkAPI.UserGroupAPI.EnterRoom enterRoom = null;

			if(connectUserList.TryGetValue(LocalUserIndex, out var user)) return null;
			LocalUser = null;

			if(string.IsNullOrWhiteSpace(roomTitle)) return null;
			if(!ThisContainer.TryGetObject<IApplicationController>(out var AppController)) return null;

			bool isConnect = await OnConnectAsync();
			async Awaitable<bool> OnConnectAsync()
			{
				return NetworkController.IsConnect ? true : await NetworkController.OnConnectAsync();
			}
			if(!isConnect) return null;

			await OnEnterAsync();
			async Awaitable OnEnterAsync()
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_ENTER_ACK>(
					new C2S_TEMP_CHATROOM_ENTER_REQ {nickname = $"{roomTitle}-{nickName}"}
				);

				if(receive == null) return;
				if(receive.Failure) return;
				if(receive.userIdx >= 0)
				{
					LocalUserIndex = receive.userIdx;
					enterRoom = new INetworkAPI.UserGroupAPI.EnterRoom();
					enterRoom.thisRoomTitle = roomTitle;
					enterRoom.thisRoomIndex = LocalUserIndex;
					enterRoom.thisUserIndex = LocalUserIndex;
				}
			}
			if(enterRoom == null)
			{
				await NetworkController.OnDisconnectAsync();
				return null;
			}
			LocalUser = CreateNetworkUserObject(LocalUserIndex, $"{roomTitle}-{nickName}");

			IEnumerable<(int userIdx, string nickname)> userList = await OnSnapshotAsync();
			async Awaitable<IEnumerable<(int userIdx, string nickname)>> OnSnapshotAsync()
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK>(new C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ {
					userIdx = LocalUserIndex
				});
				if(receive == null) return null;
				if(receive.Failure) return null;
				if(receive.UserList == null || receive.UserList.Count == 0) return null;
				return receive.UserList.Select(i => (i.userIdx, i.nickname));
			}
			UserObjectUpdate(userList);

			if(connectUserList.TryGetValue(LocalUserIndex, out user)) return enterRoom;
			else return null;
		}
		async Awaitable<INetworkAPI.UserGroupAPI.EnterRoom> OnEnterRoomAsync(string roomTitle, string nickName)
		{
			INetworkAPI.UserGroupAPI.EnterRoom enterRoom = null;

			if(connectUserList.TryGetValue(LocalUserIndex, out var user)) return null;

			if(string.IsNullOrWhiteSpace(roomTitle)) return null;
			if(!ThisContainer.TryGetObject<IApplicationController>(out var AppController)) return null;

			bool isConnect = await OnConnectAsync();
			async Awaitable<bool> OnConnectAsync()
			{
				return await NetworkController.OnConnectAsync();
			}
			if(!isConnect) return null;

			await OnEnterAsync();
			async Awaitable OnEnterAsync()
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_ENTER_ACK>(
					new C2S_TEMP_CHATROOM_ENTER_REQ {nickname = $"{roomTitle}-{nickName}"}
				);

				if(receive == null) return;
				if(receive.Failure) return;
				if(receive.userIdx >= 0)
				{
					LocalUserIndex = receive.userIdx;
					enterRoom = new INetworkAPI.UserGroupAPI.EnterRoom();
					enterRoom.thisRoomTitle = roomTitle;
					enterRoom.thisRoomIndex = LocalUserIndex;
					enterRoom.thisUserIndex = LocalUserIndex;
				}
			}
			if(enterRoom == null)
			{
				return null;
			}

			CreateNetworkUserObject(LocalUserIndex, $"{roomTitle}-{nickName}");

			IEnumerable<(int userIdx, string nickname)> userList = await OnSnapshotAsync();
			async Awaitable<IEnumerable<(int userIdx, string nickname)>> OnSnapshotAsync()
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK>(new C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ {
					userIdx = LocalUserIndex
				});
				if(receive == null) return null;
				if(receive.Failure) return null;
				if(receive.UserList == null || receive.UserList.Count == 0) return null;
				return receive.UserList.Select(i => (i.userIdx, i.nickname));
			}
			UserObjectUpdate(userList);

			if(connectUserList.TryGetValue(LocalUserIndex, out user)) return enterRoom;
			else return null;
		}
		async Awaitable OnLeaveRoomAsync()
		{
			if(LocalUserIndex<0) return;
			if(!ThisContainer.TryGetObject<IApplicationController>(out var AppController)) return;

			await OnLeaveAsync();
			async Awaitable OnLeaveAsync()
			{
				if(connectUserList.TryGetValue(LocalUserIndex, out var user))
				{
					if(user.ThisContainer.TryGetData<UserBaseData>(out var userBaseData))
					{
						userBaseData.NetworkState = UserBaseData.NetworkStateType.Disconnect;
					}
				}

				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_LEAVE_ACK>(new C2S_TEMP_CHATROOM_LEAVE_REQ { userIdx = LocalUserIndex });
				LocalUserIndex = -1;

				var keys = connectUserList.Keys.ToArray();
				int length = keys.Length;
				for(int i = 0 ; i < length ; i++)
				{
					DestroyNetworkUserObject(keys[i]);
				}
				connectUserList.Clear();
			}
		}
		async Awaitable OnUpdateUserListAsync()
		{
			IEnumerable<(int userIdx, string nickname)> userList = await OnSnapshotAync();
			async Awaitable<IEnumerable<(int userIdx, string nickname)>> OnSnapshotAync()
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK>(new C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ {
					userIdx = LocalUserIndex
				});
				if(receive == null) return null;
				if(receive.Failure) return null;
				if(receive.UserList == null || receive.UserList.Count == 0) return null;
				return receive.UserList.Select(i => (i.userIdx, i.nickname));
			}
			UserObjectUpdate(userList);
		}
		void UserEnterNty(S2C_TEMP_CHATROOM_ENTER_NTY packetData)
		{
			if(packetData.Failure) return;

			int userIdx = packetData.userIdx;
			string nickname = packetData.nickname;
			CreateNetworkUserObject(userIdx, nickname);
		}
		void UserLeaveNty(S2C_TEMP_CHATROOM_LEAVE_NTY packetData)
		{
			int userIdx = packetData.userIdx;
			if(connectUserList.TryGetValue(userIdx, out var remote))
			{
				remote.DestroyThis(true);
			}
		}
		void UserObjectUpdate(IEnumerable<(int userIdx, string nickname)> aliveUserList)
		{
			if(aliveUserList == null) return;

			// 제거되어야 하는 유저: remoteNetworkUser의 Key가 aliveUserList의 userIdx에 없는 경우
			var removeUserList = connectUserList
				.Where(remote => !aliveUserList.Any(alive => alive.userIdx == remote.Key))
				.Select(remote => remote.Key)
				.ToList();

			// 추가되어야 하는 유저: aliveUserList의 userIdx가 remoteNetworkUser의 Key에 없는 경우
			var addUserList = aliveUserList
				.Where(alive => !connectUserList.ContainsKey(alive.userIdx))
				.ToList();

			int length = removeUserList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				int userIdx = removeUserList[i];
				DestroyNetworkUserObject(removeUserList[i]);
			}

			length = addUserList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				int userIdx = addUserList[i].userIdx;
				CreateNetworkUserObject(userIdx);
			}
		}
		INetworkUser CreateNetworkUserObject(int userIdx, string nickname = "")
		{
			if(connectUserList.ContainsKey(userIdx)) return connectUserList[userIdx];

			var networkObject = ThisContainer.AddChildObject<NetworkUser>(this, false);
			var networkData = networkObject.ThisContainer.AddData<UserBaseData>();
			networkData.IsLocal = userIdx == LocalUserIndex;
			networkData.UserIdx = userIdx;
			networkData.Nickname = string.IsNullOrWhiteSpace(nickname) ? $"Nickname{userIdx:00}" : nickname;
			networkData.NetworkState = UserBaseData.NetworkStateType.Connect;
			networkObject.GameObject.SetActive(true);

			connectUserList.Add(userIdx, networkObject);
			return networkObject;
		}
		void DestroyNetworkUserObject(int userIdx)
		{
			if(!connectUserList.TryGetValue(userIdx, out var leaveUser)) return;
			if(leaveUser.ThisContainer.TryGetData<UserBaseData>(out var userBaseData))
			{
				userBaseData.NetworkState = UserBaseData.NetworkStateType.Disconnect;
			}
			connectUserList.Remove(userIdx);
			leaveUser?.DestroyThis(true);
		}
		bool TryGetNetworkUser(int userIndex, out INetworkUser networkUser)
		{
			return connectUserList.TryGetValue(userIndex, out networkUser);
		}


		async Awaitable<INetworkAPI.UserGroupAPI.EnterRoom> INetworkAPI.UserGroupAPI.OnCreateRoomAsync(string roomTitle, string nickName) => await OnCreateRoomAsync(roomTitle, nickName);
		async Awaitable<INetworkAPI.UserGroupAPI.EnterRoom> INetworkAPI.UserGroupAPI.OnEnterRoomAsync(string roomTitle, string nickName) => await OnEnterRoomAsync(roomTitle, nickName);
		async Awaitable INetworkAPI.UserGroupAPI.OnLeaveRoomAsync() => await OnLeaveRoomAsync();
		async Awaitable INetworkAPI.UserGroupAPI.OnUpdateUserListAsync() => await OnUpdateUserListAsync();
		bool INetworkAPI.UserGroupAPI.TryGetNetworkUser(int userIndex, out INetworkUser networkUser) => TryGetNetworkUser(userIndex, out networkUser);

	}
}