using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ws_json_client
{
    // Base interface for packet
    public interface IPacket
    {
        int Code { get; }
        JsonElement ToJson();
    }

    // C2S Packets
    public class C2S_TEMP_CHATROOM_ENTER_REQ : IPacket
    {
        public int Code => (int)C2S.C2S_TEMP_CHATROOM_ENTER_REQ;
        public string Nickname { get; set; }

        public C2S_TEMP_CHATROOM_ENTER_REQ(string nickname)
        {
            Nickname = nickname;
        }

        public static C2S_TEMP_CHATROOM_ENTER_REQ FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in C2S_TEMP_CHATROOM_ENTER_REQ");

            if (!actualData.TryGetProperty("nickname", out JsonElement nicknameElement))
                throw new ArgumentException("Missing 'nickname' key in data");

            return new C2S_TEMP_CHATROOM_ENTER_REQ(nicknameElement.GetString());
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new { nickname = Nickname }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    public class C2S_TEMP_CHATROOM_LEAVE_REQ : IPacket
    {
        public int Code => (int)C2S.C2S_TEMP_CHATROOM_LEAVE_REQ;
        public int UserIdx { get; set; }

        public C2S_TEMP_CHATROOM_LEAVE_REQ(int userIdx)
        {
            UserIdx = userIdx;
        }

        public static C2S_TEMP_CHATROOM_LEAVE_REQ FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in C2S_TEMP_CHATROOM_LEAVE_REQ");

            if (!actualData.TryGetProperty("userIdx", out JsonElement userIdxElement))
                throw new ArgumentException("Missing 'userIdx' key in data");

            return new C2S_TEMP_CHATROOM_LEAVE_REQ(userIdxElement.GetInt32());
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new { userIdx = UserIdx }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    public class C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ : IPacket
    {
        public int Code => (int)C2S.C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ;
        public int UserIdx { get; set; }

        public C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ(int userIdx)
        {
            UserIdx = userIdx;
        }

        public static C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ");

            if (!actualData.TryGetProperty("userIdx", out JsonElement userIdxElement))
                throw new ArgumentException("Missing 'userIdx' key in data");

            return new C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ(userIdxElement.GetInt32());
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new { userIdx = UserIdx }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    public class C2S_TEMP_CHATROOM_CHAT_SEND_REQ : IPacket
    {
        public int Code => (int)C2S.C2S_TEMP_CHATROOM_CHAT_SEND_REQ;
        public int UserIdx { get; set; }
        public string Chat { get; set; }

        public C2S_TEMP_CHATROOM_CHAT_SEND_REQ(int userIdx, string chat)
        {
            UserIdx = userIdx;
            Chat = chat;
        }

        public static C2S_TEMP_CHATROOM_CHAT_SEND_REQ FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in C2S_TEMP_CHATROOM_CHAT_SEND_REQ");

            if (!actualData.TryGetProperty("userIdx", out JsonElement userIdxElement) ||
                !actualData.TryGetProperty("chat", out JsonElement chatElement))
                throw new ArgumentException("Missing required keys in data");

            return new C2S_TEMP_CHATROOM_CHAT_SEND_REQ(userIdxElement.GetInt32(), chatElement.GetString());
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new { userIdx = UserIdx, chat = Chat }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    // S2C Packets
    public class S2C_TEMP_CHATROOM_ENTER_ACK : IPacket
    {
        public int Code => (int)S2C.S2C_TEMP_CHATROOM_ENTER_ACK;
        public int Result { get; set; }
        public int UserIdx { get; set; }
        public string Nickname { get; set; }

        public S2C_TEMP_CHATROOM_ENTER_ACK(int result, int userIdx, string nickname)
        {
            Result = result;
            UserIdx = userIdx;
            Nickname = nickname;
        }

        public static S2C_TEMP_CHATROOM_ENTER_ACK FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in S2C_TEMP_CHATROOM_ENTER_ACK");

            if (!actualData.TryGetProperty("result", out JsonElement resultElement) ||
                !actualData.TryGetProperty("userIdx", out JsonElement userIdxElement) ||
                !actualData.TryGetProperty("nickname", out JsonElement nicknameElement))
                throw new ArgumentException("Missing required keys in data");

            return new S2C_TEMP_CHATROOM_ENTER_ACK(resultElement.GetInt32(), userIdxElement.GetInt32(), nicknameElement.GetString());
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new
                {
                    result = Result,
                    userIdx = UserIdx,
                    nickname = Nickname
                }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    public class S2C_TEMP_CHATROOM_ENTER_NTY : IPacket
    {
        public int Code => (int)S2C.S2C_TEMP_CHATROOM_ENTER_NTY;
        public int UserIdx { get; set; }
        public string Nickname { get; set; }

        public S2C_TEMP_CHATROOM_ENTER_NTY(int userIdx, string nickname)
        {
            UserIdx = userIdx;
            Nickname = nickname;
        }

        public static S2C_TEMP_CHATROOM_ENTER_NTY FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in S2C_TEMP_CHATROOM_ENTER_NTY");

            if (!actualData.TryGetProperty("userIdx", out JsonElement userIdxElement) ||
                !actualData.TryGetProperty("nickname", out JsonElement nicknameElement))
                throw new ArgumentException("Missing required keys in data");

            return new S2C_TEMP_CHATROOM_ENTER_NTY(userIdxElement.GetInt32(), nicknameElement.GetString());
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new { userIdx = UserIdx, nickname = Nickname }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    public class S2C_TEMP_CHATROOM_LEAVE_ACK : IPacket
    {
        public int Code => (int)S2C.S2C_TEMP_CHATROOM_LEAVE_ACK;
        public int Result { get; set; }

        public S2C_TEMP_CHATROOM_LEAVE_ACK(int result)
        {
            Result = result;
        }

        public static S2C_TEMP_CHATROOM_LEAVE_ACK FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in S2C_TEMP_CHATROOM_LEAVE_ACK");

            if (!actualData.TryGetProperty("result", out JsonElement resultElement))
                throw new ArgumentException("Missing 'result' key in data");

            return new S2C_TEMP_CHATROOM_LEAVE_ACK(resultElement.GetInt32());
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new { result = Result }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    public class S2C_TEMP_CHATROOM_LEAVE_NTY : IPacket
    {
        public int Code => (int)S2C.S2C_TEMP_CHATROOM_LEAVE_NTY;
        public int UserIdx { get; set; }

        public S2C_TEMP_CHATROOM_LEAVE_NTY(int userIdx)
        {
            UserIdx = userIdx;
        }

        public static S2C_TEMP_CHATROOM_LEAVE_NTY FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in S2C_TEMP_CHATROOM_LEAVE_NTY");

            if (!actualData.TryGetProperty("userIdx", out JsonElement userIdxElement))
                throw new ArgumentException("Missing 'userIdx' key in data");

            return new S2C_TEMP_CHATROOM_LEAVE_NTY(userIdxElement.GetInt32());
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new { userIdx = UserIdx }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    public class S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK : IPacket
    {
        public int Code => (int)S2C.S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK;
        public int Result { get; set; }
        public List<(int UserIdx, string Nickname)> UserList { get; set; }

        public S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK(int result, List<(int, string)> userList)
        {
            Result = result;
            UserList = userList;
        }

        public static S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK");

            if (!actualData.TryGetProperty("result", out JsonElement resultElement) ||
                !actualData.TryGetProperty("userList", out JsonElement userListElement))
                throw new ArgumentException("Missing required keys in data");

            var userList = new List<(int, string)>();
            foreach (JsonElement user in userListElement.EnumerateArray())
            {
                if (!user.TryGetProperty("userIdx", out JsonElement userIdxElement) ||
                    !user.TryGetProperty("nickname", out JsonElement nicknameElement))
                    throw new ArgumentException("Each user in 'userList' must contain 'userIdx' and 'nickname' keys");

                userList.Add((userIdxElement.GetInt32(), nicknameElement.GetString()));
            }

            return new S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK(resultElement.GetInt32(), userList);
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new
                {
                    result = Result,
                    userList = UserList.ConvertAll(user => new { userIdx = user.UserIdx, nickname = user.Nickname })
                }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    public class S2C_TEMP_CHATROOM_CHAT_SEND_ACK : IPacket
    {
        public int Code => (int)S2C.S2C_TEMP_CHATROOM_CHAT_SEND_ACK;
        public int Result { get; set; }
        public int ChatIdx { get; set; }
        public string Chat { get; set; }
        public long UtcMs { get; set; }

        public S2C_TEMP_CHATROOM_CHAT_SEND_ACK(int result, int chatIdx, string chat, long utcMs)
        {
            Result = result;
            ChatIdx = chatIdx;
            Chat = chat;
            UtcMs = utcMs;
        }

        public static S2C_TEMP_CHATROOM_CHAT_SEND_ACK FromJson(JsonElement data)
        {
            Console.WriteLine(data);
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in S2C_TEMP_CHATROOM_CHAT_SEND_ACK");

            if (!actualData.TryGetProperty("result", out JsonElement resultElement) ||
                !actualData.TryGetProperty("chatIdx", out JsonElement chatIdxElement) ||
                !actualData.TryGetProperty("chat", out JsonElement chatElement) ||
                !actualData.TryGetProperty("utcMs", out JsonElement utcMsElement))
                throw new ArgumentException("Missing required keys in data");

            return new S2C_TEMP_CHATROOM_CHAT_SEND_ACK(
                resultElement.GetInt32(),
                chatIdxElement.GetInt32(),
                chatElement.GetString(),
                utcMsElement.GetInt64()
            );
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new
                {
                    result = Result,
                    chatIdx = ChatIdx,
                    chat = Chat,
                    utcMs = UtcMs
                }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }

    public class S2C_TEMP_CHATROOM_CHAT_SEND_NTY : IPacket
    {
        public int Code => (int)S2C.S2C_TEMP_CHATROOM_CHAT_SEND_NTY;
        public int UserIdx { get; set; }
        public int ChatIdx { get; set; }
        public string Chat { get; set; }
        public long UtcMs { get; set; }

        public S2C_TEMP_CHATROOM_CHAT_SEND_NTY(int userIdx, int chatIdx, string chat, long utcMs)
        {
            UserIdx = userIdx;
            ChatIdx = chatIdx;
            Chat = chat;
            UtcMs = utcMs;
        }

        public static S2C_TEMP_CHATROOM_CHAT_SEND_NTY FromJson(JsonElement data)
        {
            if (!data.TryGetProperty("data", out JsonElement actualData))
                throw new ArgumentException("Missing 'data' key in S2C_TEMP_CHATROOM_CHAT_SEND_NTY");

            if (!actualData.TryGetProperty("userIdx", out JsonElement userIdxElement) ||
                !actualData.TryGetProperty("chatIdx", out JsonElement chatIdxElement) ||
                !actualData.TryGetProperty("chat", out JsonElement chatElement) ||
                !actualData.TryGetProperty("utcMs", out JsonElement utcMsElement))
                throw new ArgumentException("Missing required keys in data");

            return new S2C_TEMP_CHATROOM_CHAT_SEND_NTY(
                userIdxElement.GetInt32(),
                chatIdxElement.GetInt32(),
                chatElement.GetString(),
                utcMsElement.GetInt64()
            );
        }

        public JsonElement ToJson()
        {
            var json = new
            {
                code = Code,
                data = new
                {
                    userIdx = UserIdx,
                    chatIdx = ChatIdx,
                    chat = Chat,
                    utcMs = UtcMs
                }
            };
            return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(json));
        }
    }
}
