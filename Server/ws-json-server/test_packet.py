import unittest
from protocol import C2S, S2C

# Importing the classes to be tested
from packet import (
    C2S_TEMP_CHATROOM_ENTER_REQ,
    C2S_TEMP_CHATROOM_LEAVE_REQ,
    C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ,
    C2S_TEMP_CHATROOM_CHAT_SEND_REQ,
    S2C_TEMP_CHATROOM_ENTER_ACK,
    S2C_TEMP_CHATROOM_ENTER_NTY,
    S2C_TEMP_CHATROOM_LEAVE_ACK,
    S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK,
    S2C_TEMP_CHATROOM_LEAVE_NTY,
    S2C_TEMP_CHATROOM_CHAT_SEND_ACK,
    S2C_TEMP_CHATROOM_CHAT_SEND_NTY,
)

class TestChatroomPackets(unittest.TestCase):
    def test_C2S_TEMP_CHATROOM_ENTER_REQ(self):
        original_data = {"code": C2S.C2S_TEMP_CHATROOM_ENTER_REQ.value, "data": {"nickname": "testuser"}}
        packet = C2S_TEMP_CHATROOM_ENTER_REQ.from_dict(original_data)
        self.assertEqual(packet.nickname, "testuser")
        self.assertEqual(packet.to_dict(), original_data)

    def test_C2S_TEMP_CHATROOM_LEAVE_REQ(self):
        original_data = {"code": C2S.C2S_TEMP_CHATROOM_LEAVE_REQ.value, "data": {"userIdx": 123}}
        packet = C2S_TEMP_CHATROOM_LEAVE_REQ.from_dict(original_data)
        self.assertEqual(packet.user_idx, 123)
        self.assertEqual(packet.to_dict(), original_data)

    def test_C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ(self):
        original_data = {"code": C2S.C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ.value, "data": {"userIdx": 456}}
        packet = C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ.from_dict(original_data)
        self.assertEqual(packet.user_idx, 456)
        self.assertEqual(packet.to_dict(), original_data)

    def test_C2S_TEMP_CHATROOM_CHAT_SEND_REQ(self):
        original_data = {"code": C2S.C2S_TEMP_CHATROOM_CHAT_SEND_REQ.value, "data": {"userIdx": 789, "chat": "Hello World"}}
        packet = C2S_TEMP_CHATROOM_CHAT_SEND_REQ.from_dict(original_data)
        self.assertEqual(packet.user_idx, 789)
        self.assertEqual(packet.chat, "Hello World")
        self.assertEqual(packet.to_dict(), original_data)

    def test_S2C_TEMP_CHATROOM_ENTER_ACK(self):
        original_data = {"code": S2C.S2C_TEMP_CHATROOM_ENTER_ACK.value, "data": {"result": "0", "userIdx": 123, "nickname": "testuser"}}
        packet = S2C_TEMP_CHATROOM_ENTER_ACK.from_dict(original_data)
        self.assertEqual(packet.result, "0")
        self.assertEqual(packet.user_idx, 123)
        self.assertEqual(packet.nickname, "testuser")
        self.assertEqual(packet.to_dict(), original_data)

    def test_S2C_TEMP_CHATROOM_ENTER_NTY(self):
        original_data = {"code": S2C.S2C_TEMP_CHATROOM_ENTER_NTY.value, "data": {"userIdx": 456, "nickname": "anotheruser"}}
        packet = S2C_TEMP_CHATROOM_ENTER_NTY.from_dict(original_data)
        self.assertEqual(packet.user_idx, 456)
        self.assertEqual(packet.nickname, "anotheruser")
        self.assertEqual(packet.to_dict(), original_data)

    def test_S2C_TEMP_CHATROOM_LEAVE_ACK(self):
        original_data = {"code": S2C.S2C_TEMP_CHATROOM_LEAVE_ACK.value, "data": {"result": "0"}}
        packet = S2C_TEMP_CHATROOM_LEAVE_ACK.from_dict(original_data)
        self.assertEqual(packet.result, "0")
        self.assertEqual(packet.to_dict(), original_data)

    def test_S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK(self):
        original_data = {
            "code": S2C.S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK.value,
            "data": {
                "result": "0",
                "userList": [
                    {"userIdx": 1, "nickname": "user1"},
                    {"userIdx": 2, "nickname": "user2"},
                ],
            },
        }
        packet = S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK.from_dict(original_data)
        self.assertEqual(packet.result, "0")
        self.assertEqual(len(packet.user_list), 2)
        self.assertEqual(packet.user_list[0]["userIdx"], 1)
        self.assertEqual(packet.user_list[0]["nickname"], "user1")
        self.assertEqual(packet.to_dict(), original_data)

    def test_S2C_TEMP_CHATROOM_LEAVE_NTY(self):
        original_data = {"code": S2C.S2C_TEMP_CHATROOM_LEAVE_NTY.value, "data": {"userIdx": 789}}
        packet = S2C_TEMP_CHATROOM_LEAVE_NTY.from_dict(original_data)
        self.assertEqual(packet.user_idx, 789)
        self.assertEqual(packet.to_dict(), original_data)

    def test_S2C_TEMP_CHATROOM_CHAT_SEND_ACK(self):
        original_data = {
            "code": S2C.S2C_TEMP_CHATROOM_CHAT_SEND_ACK.value,
            "data": {
                "result": 0,
                "chatIdx": 100,
                "chat": "Hello!",
                "utcMs": 1627889182736,
            },
        }
        packet = S2C_TEMP_CHATROOM_CHAT_SEND_ACK.from_dict(original_data)
        self.assertEqual(packet.result, 0)
        self.assertEqual(packet.chat_idx, 100)
        self.assertEqual(packet.chat, "Hello!")
        self.assertEqual(packet.utc_ms, 1627889182736)
        self.assertEqual(packet.to_dict(), original_data)

    def test_S2C_TEMP_CHATROOM_CHAT_SEND_NTY(self):
        original_data = {
            "code": S2C.S2C_TEMP_CHATROOM_CHAT_SEND_NTY.value,
            "data": {
                "userIdx": 101,
                "chatIdx": 200,
                "chat": "Goodbye!",
                "utcMs": 1627889182737,
            },
        }
        packet = S2C_TEMP_CHATROOM_CHAT_SEND_NTY.from_dict(original_data)
        self.assertEqual(packet.user_idx, 101)
        self.assertEqual(packet.chat_idx, 200)
        self.assertEqual(packet.chat, "Goodbye!")
        self.assertEqual(packet.utc_ms, 1627889182737)
        self.assertEqual(packet.to_dict(), original_data)

if __name__ == "__main__":
    unittest.main()
