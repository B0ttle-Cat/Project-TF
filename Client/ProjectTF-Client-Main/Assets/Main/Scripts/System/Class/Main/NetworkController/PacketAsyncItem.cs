using UnityEngine;

namespace TFSystem
{
	using System;

	using BC.Base;

	using TFSystem.Network;

	public class PacketAsyncItem
	{
		private class ReceiveHandler<T> : INetworkReceiveHandler<T> where T : class, IPacketReceive
		{
			private AwaitableCompletionSource<T> onReceive;
			public ReceiveHandler(AwaitableCompletionSource<T> onReceive)
			{
				this.onReceive=onReceive;
				EventManager.AddListener(this);
			}

			public void OnReceive(T packetData)
			{
				onReceive.SetResult(packetData);
			}
			public void Dispose()
			{
				onReceive = null;
				EventManager.RemoveListener(this);
			}
		}

		public static async Awaitable<TReceive> OnSendReceiveAsync<TReceive>(IPacketSend sendData) where TReceive : class, IPacketReceive
		{
			if(sendData == null) return null;
			AwaitableCompletionSource<TReceive> receiveData = new AwaitableCompletionSource<TReceive>();
			ReceiveHandler<TReceive> handler = new ReceiveHandler<TReceive>(receiveData);
			await EventManager.Call<INetworkSendEvent>(call => call.OnSendAsync(sendData));
			var receive = await receiveData.Awaitable;
			receiveData.Reset();
			handler.Dispose();
			return receive;
		}

		public static PacketNotifyItem CreateNotifyItem<TReceive>(Action<TReceive> onNotify, bool sleep = false) where TReceive : class, IPacketReceive
		{
			return new PacketNotifyItem<TReceive>(onNotify, sleep);
		}
	}
	public abstract class PacketNotifyItem : IDisposable
	{
		public abstract bool sleep { get; set; }
		public abstract void Dispose();
	}
	public class PacketNotifyItem<TReceive> : PacketNotifyItem, INetworkReceiveHandler<TReceive>
		where TReceive : class, IPacketReceive
	{
		private Action<TReceive> onNotify;
		public override bool sleep {
			get {
				if(onNotify == null) return true;
				return !EventManager.Contains(this);
			}
			set {
				if(onNotify == null) return;
				else if(value && EventManager.Contains(this))
				{
					EventManager.RemoveListener(this);
				}
				else if(!value && !EventManager.Contains(this))
				{
					EventManager.AddListener(this);
				}
			}
		}
		internal PacketNotifyItem(Action<TReceive> onNotify, bool sleep)
		{
			this.onNotify = onNotify;
			this.sleep = sleep;
		}

		void INetworkReceiveHandler<TReceive>.OnReceive(TReceive packetData)
		{
			onNotify?.Invoke(packetData);
		}

		public override void Dispose()
		{
			onNotify = null;
			sleep = true;
		}
	}
}
