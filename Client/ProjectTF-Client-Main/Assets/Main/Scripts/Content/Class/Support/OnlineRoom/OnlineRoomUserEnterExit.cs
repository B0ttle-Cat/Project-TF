using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.ODCC;

using TFSystem;
using TFSystem.Network;

using UnityEngine;
namespace TFContent
{
	public interface IOnlineRoomUserListUpdate
	{
		void OnUserListUpdate(List<(int userIdx, string nickname)> userList);
		void OnEnterUser(int userIdx, string nickname);
		void OnLeaveUser(int userIdx);
	}


	public class OnlineRoomUserEnterExit : ComponentBehaviour//, IOdccUpdate
	{
		PacketNotifyItem userEnterNty;
		PacketNotifyItem userLeaveNty;
		///Awake 대신 사용.
		protected override void BaseAwake()
		{
			userEnterNty = PacketAsyncItem.CreateNotifyItem<S2C_TEMP_CHATROOM_ENTER_NTY>(UserEnterNty, true);
			userLeaveNty = PacketAsyncItem.CreateNotifyItem<S2C_TEMP_CHATROOM_LEAVE_NTY>(UserLeaveNty, true);
		}
		///OnEnable 대신 사용.
		protected override async void BaseEnable()
		{
			userEnterNty.sleep = false;
			userLeaveNty.sleep = false;
			await OnSnapshot();
		}
		///Start 대신 사용.
		protected override void BaseStart()
		{

		}
		///OnDisable 대신 사용.
		protected override void BaseDisable()
		{
			userEnterNty.sleep = true;
			userLeaveNty.sleep = true;
		}
		///OnDestroy 대신 사용
		protected override void BaseDestroy()
		{
			userEnterNty.Dispose();
			userEnterNty = null;
		}
		private async Awaitable OnSnapshot()
		{
			if(ThisContainer.TryGetObject<SystemState>(out var systemState))
			{
				systemState.AppController.DataCarrier.GetData("userIdx", out int userIdx, -1);
				if(userIdx>=0)
				{

					var snapshot = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK>(new C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ {
						userIdx = userIdx
					});
					if(snapshot != null && snapshot.Succeed)
					{
						var userList = snapshot.UserList.Select(i=>(i.userIdx,i.nickname)).ToList();
						EventManager.Call<IOnlineRoomUserListUpdate>(x => x.OnUserListUpdate(userList));
					}
				}
			}
		}
		private void UserEnterNty(S2C_TEMP_CHATROOM_ENTER_NTY packet)
		{
			EventManager.Call<IOnlineRoomUserListUpdate>(x => x.OnEnterUser(packet.userIdx, packet.nickname));
		}
		private void UserLeaveNty(S2C_TEMP_CHATROOM_LEAVE_NTY packet)
		{
			EventManager.Call<IOnlineRoomUserListUpdate>(x => x.OnLeaveUser(packet.userIdx));
		}
	}
}