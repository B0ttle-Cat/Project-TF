using System;
using System.Collections.Generic;

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
		static T FromJson<T>(in string json) where T : class, IPacketReceive
		{
			var jsonObject = new {
				code = default(int),
				data = default(T),
			};
			jsonObject = JsonConvert.DeserializeAnonymousType(json, jsonObject);

			return jsonObject.data;
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
		public List<User> UserList;
		public struct User
		{
			public int userIdx;
			public string nickname;
		}
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