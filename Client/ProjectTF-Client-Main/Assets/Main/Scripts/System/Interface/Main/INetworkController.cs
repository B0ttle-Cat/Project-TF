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