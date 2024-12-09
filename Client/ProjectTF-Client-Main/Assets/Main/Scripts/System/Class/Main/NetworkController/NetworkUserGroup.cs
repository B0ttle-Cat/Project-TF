using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using UnityEngine;

namespace TFSystem.Network
{
	public class NetworkUserGroup : ComponentBehaviour
	{
		int localNetworkIndex;
		INetworkUser localNetworkUser;
		Dictionary<int, INetworkUser> connectUserList;

		PacketNotifyItem userEnterNty;
		PacketNotifyItem userLeaveNty;
		protected override void BaseAwake()
		{
			userEnterNty = PacketAsyncItem.CreateNotifyItem<S2C_TEMP_CHATROOM_ENTER_NTY>(UserEnterNty, true);
			userLeaveNty = PacketAsyncItem.CreateNotifyItem<S2C_TEMP_CHATROOM_LEAVE_NTY>(UserLeaveNty, true);

			connectUserList = new Dictionary<int, INetworkUser>();
		}
		protected override void BaseDestroy()
		{
			userEnterNty?.Dispose();
			userLeaveNty?.Dispose();
			connectUserList?.Clear();

			localNetworkIndex = -1;

			userEnterNty  = null;
			userLeaveNty  = null;
			connectUserList = null;
		}

		public async Awaitable<int> OnEnterRoomAsync(string nickname)
		{
			if(localNetworkIndex>=0) return localNetworkIndex;

			if(string.IsNullOrWhiteSpace(nickname)) return -1;
			if(!ThisContainer.TryGetObject<IApplicationController>(out var AppController)) return -1;

			bool isConnect = await OnConnectAsync();
			async Awaitable<bool> OnConnectAsync()
			{
				return await AppController.NetworkController.OnConnectAsync();
			}
			if(!isConnect) return -1;

			localNetworkIndex = await OnEnterAsync();
			async Awaitable<int> OnEnterAsync()
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_ENTER_ACK>(
						new C2S_TEMP_CHATROOM_ENTER_REQ {nickname = nickname}
				);

				if(receive == null) return -1;
				if(receive.Failure) return -1;
				if(receive.userIdx < 0)
				{
					await AppController.NetworkController.OnDisconnectAsync();
				}
				return receive.userIdx;
			}
			if(localNetworkIndex < 0) return -1;

			localNetworkUser = CreateNetworkUserObject(localNetworkIndex, nickname);

			IEnumerable<(int userIdx, string nickname)> userList = await OnSnapshotAync();
			async Awaitable<IEnumerable<(int userIdx, string nickname)>> OnSnapshotAync()
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK>(new C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ {
					userIdx = localNetworkIndex
				});
				if(receive == null) return null;
				if(receive.Failure) return null;
				if(receive.UserList == null || receive.UserList.Count == 0) return null;
				return receive.UserList.Select(i => (i.userIdx, i.nickname));
			}
			UserObjectUpdate(userList);

			return localNetworkIndex;
		}
		public async Awaitable OnLeaveRoomAsync()
		{
			if(localNetworkIndex<0) return;
			if(!ThisContainer.TryGetObject<IApplicationController>(out var AppController)) return;

			await OnLeaveAsync();
			async Awaitable OnLeaveAsync()
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_LEAVE_ACK>(new C2S_TEMP_CHATROOM_LEAVE_REQ { userIdx = localNetworkIndex });
				localNetworkIndex = -1;

				var keys = connectUserList.Keys.ToArray();
				int length = keys.Length;
				for(int i = 0 ; i < length ; i++)
				{
					DestroyNetworkUserObject(keys[i]);
				}
				connectUserList.Clear();
			}

			await OnDisconnectAsync();
			async Awaitable OnDisconnectAsync()
			{
				await AppController.NetworkController.OnDisconnectAsync();
			}
		}

		INetworkUser CreateNetworkUserObject(int userIdx, string nickname)
		{
			if(connectUserList.ContainsKey(userIdx)) return connectUserList[userIdx];

			var networkObject = ThisContainer.AddChildObject<ObjectBehaviour>(this, false);
			var networkUser = networkObject.ThisContainer.AddComponent<NetworkUser>();
			var networkData = networkObject.ThisContainer.AddData<UserBaseData>();
			networkData.IsLocal = userIdx == localNetworkIndex;
			networkData.UserIdx = userIdx;
			networkData.Nickname = nickname;
			networkData.NetworkState = UserBaseData.NetworkStateType.Connect;
			networkUser.GameObject.SetActive(true);

			connectUserList.Add(userIdx, networkUser);
			return networkUser;
		}
		void DestroyNetworkUserObject(int userIdx)
		{
			if(!connectUserList.TryGetValue(userIdx, out var leaveUser)) return;
			connectUserList.Remove(userIdx);
			leaveUser?.DestroyThis(true);
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
			if(localNetworkIndex >= 0)
			{
				aliveUserList = aliveUserList.Where(i => i.userIdx != localNetworkIndex);
			}

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
				string nickname = addUserList[i].nickname;
				CreateNetworkUserObject(userIdx, nickname);
			}
		}
	}
}