using System;
using System.Collections.Generic;


using UnityEngine;

namespace TFSystem
{
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

namespace TFSystem.Network
{
	using Newtonsoft.Json;

	using static TFSystem.Network.Protocol;

	#region Interface
	public interface IPacket
	{
		int Code { get; }
	}
	public interface IPacketSend : IPacket
	{
		static string ToJson(IPacketSend _this)
		{
			return JsonConvert.SerializeObject(new {
				code = _this.Code,
				data = _this
			});
		}
	}
	public interface IPacketReceive : IPacket
	{
		bool Succeed { get; }
		bool Failure { get; }
		static bool TryS2CCode(in string json, out S2C _S2C)
		{
			try
			{
				var jsonObject = new { code = 0 };
				jsonObject = JsonConvert.DeserializeAnonymousType(json, jsonObject);
				if(Enum.IsDefined(typeof(S2C), jsonObject.code))
				{
					_S2C = (S2C)jsonObject.code;
					return true;
				}
				else
				{
					_S2C = (S2C)0;
					return false;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				_S2C = (S2C)0;
				return false;
			}
		}
		static T FromJson<T>(in string json) where T : class, IPacketReceive
		{
			try
			{
				var jsonObject = new {
					code = default(int),
					data = default(T),
				};
				jsonObject = JsonConvert.DeserializeAnonymousType(json, jsonObject);

				return jsonObject.data;
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return null;
			}
		}
	}
	#endregion

	#region C2S
	public class C2S_TEMP_CHATROOM_ENTER_REQ : IPacketSend
	{
		public int Code => (int)C2S.C2S_TEMP_CHATROOM_ENTER_REQ;
		public string nickname;
	}
	public class C2S_TEMP_CHATROOM_LEAVE_REQ : IPacketSend
	{
		public int Code => (int)C2S.C2S_TEMP_CHATROOM_LEAVE_REQ;
		public int userIdx;
	}
	public class C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ : IPacketSend
	{
		public int Code => (int)C2S.C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ;
		public int userIdx;

	}
	public class C2S_TEMP_CHATROOM_CHAT_SEND_REQ : IPacketSend
	{
		public int Code => (int)C2S.C2S_TEMP_CHATROOM_CHAT_SEND_REQ;
		public int userIdx;
		public string chat;
	}
	#endregion

	#region S2C
	public class S2C_TEMP_CHATROOM_ENTER_ACK : IPacketReceive
	{
		public int Code => (int)S2C.S2C_TEMP_CHATROOM_ENTER_ACK;
		public bool Succeed => result == (int)Result.SUCCEED;
		public bool Failure => result != (int)Result.SUCCEED;

		public int result;
		public int userIdx;
		public string nickname;
	}
	public class S2C_TEMP_CHATROOM_ENTER_NTY : IPacketReceive
	{
		public int Code => (int)S2C.S2C_TEMP_CHATROOM_ENTER_NTY;
		public bool Succeed => true;
		public bool Failure => false;

		public int userIdx;
		public string nickname;
	}
	public class S2C_TEMP_CHATROOM_LEAVE_ACK : IPacketReceive
	{
		public int Code => (int)S2C.S2C_TEMP_CHATROOM_LEAVE_ACK;
		public bool Succeed => result == (int)Result.SUCCEED;
		public bool Failure => result != (int)Result.SUCCEED;

		public int result;
	}
	public class S2C_TEMP_CHATROOM_LEAVE_NTY : IPacketReceive
	{
		public int Code => (int)S2C.S2C_TEMP_CHATROOM_LEAVE_NTY;
		public bool Succeed => true;
		public bool Failure => false;

		public int userIdx;
	}
	public class S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK : IPacketReceive
	{
		public int Code => (int)S2C.S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK;
		public bool Succeed => result == (int)Result.SUCCEED;
		public bool Failure => result != (int)Result.SUCCEED;

		public int result;
		public List<(int userIdx, string nickname)> UserList;
	}
	public class S2C_TEMP_CHATROOM_CHAT_SEND_ACK : IPacketReceive
	{
		public int Code => (int)S2C.S2C_TEMP_CHATROOM_CHAT_SEND_ACK;
		public bool Succeed => result == (int)Result.SUCCEED;
		public bool Failure => result != (int)Result.SUCCEED;

		public int result;
		public int chatIdx;
		public string Chat;
		public long UtcMs;
	}
	public class S2C_TEMP_CHATROOM_CHAT_SEND_NTY : IPacketReceive
	{
		public int Code => (int)S2C.S2C_TEMP_CHATROOM_CHAT_SEND_NTY;
		public bool Succeed => true;
		public bool Failure => false;

		public int userIdx;
		public int chatIdx;
		public string chat;
		public long utcMs;
	}
	#endregion
}