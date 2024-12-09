using BC.ODCC;

using TFSystem.Network;

using UnityEngine;
namespace TFSystem
{
	public interface INetworkController : IOdccComponent
	{
		string NetworkIp { get; }
		string NetworkPort { get; }
		string NetworkURL { get; }
		Awaitable<bool> OnConnectAsync();
		Awaitable OnDisconnectAsync();

		INetworkAPI.UserGroupAPI UserGroupAPI { get; }
	}
	public interface INetworkAPI : IOdccComponent
	{
		public INetworkController NetworkController { get; }
		public interface UserGroupAPI : INetworkAPI
		{
			public class EnterRoom
			{
				public string thisRoomTitle;
				public int thisRoomIndex;
				public int thisUserIndex;
			}
			int LocalUserIndex { get; }
			Awaitable<EnterRoom> OnCreateRoomAsync(string roomTitle, string nickName);
			Awaitable<EnterRoom> OnEnterRoomAsync(string roomTitle, string nickName);
			Awaitable OnLeaveRoomAsync();
			Awaitable OnUpdateUserListAsync();
			bool TryGetNetworkUser(int userIndex, out INetworkUser networkUser);
		}
	}
	public interface INetworkSendEvent
	{
		Awaitable OnSendAsync<T>(T packetData) where T : class, IPacketSend;
	}
	public interface INetworkReceiveHandler<T> where T : class, IPacketReceive
	{
		void OnReceive(T packetData);
	}

}