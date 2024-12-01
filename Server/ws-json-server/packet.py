from protocol import C2S, S2C, Result

class C2S_TEMP_CHATROOM_CHAT_SEND_REQ:
    code = C2S.C2S_TEMP_CHATROOM_CHAT_SEND_REQ.value

    def __init__(self, user_idx: int = 0, chat: str = ""):
        self.user_idx = user_idx
        self.chat = chat

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing actual data in C2S_TEMP_CHATROOM_CHAT_SEND_REQ")
        if not isinstance(data['data'], dict):
            raise ValueError("Actual data value must be dictionary")
        
        actual_data = data['data']

        if 'userIdx' not in actual_data:
            raise ValueError("Missing 'userIdx' key in data of C2S_TEMP_CHATROOM_CHAT_SEND_REQ")
        if not isinstance(actual_data['userIdx'], int):
            raise ValueError("The 'userIdx' value must be an integer")
        if 'chat' not in actual_data:
            raise ValueError("Missing 'chat' key in data of C2S_TEMP_CHATROOM_CHAT_SEND_REQ")
        if not isinstance(actual_data['chat'], str):
            raise ValueError("The 'chat' value must be an string")

        return cls(
            user_idx = actual_data['userIdx'],
            chat = actual_data['chat']
        )

    def to_dict(self):
        return {
            "userIdx": self.user_idx,
            "chat": self.chat
        }
    
class S2C_TEMP_CHATROOM_CHAT_SEND_ACK:
    code = S2C.S2C_TEMP_CHATROOM_CHAT_SEND_ACK.value

    def __init__(self, result: int, chat_idx: int, chat: str, utc_ms: int):
        self.result = result
        self.chat_idx = chat_idx
        self.chat = chat
        self.utc_ms = utc_ms

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing actual data in S2C_TEMP_CHATROOM_CHAT_SEND_ACK")
        if not isinstance(data['data'], dict):
            raise ValueError("Actual data value must be dictionary")
        
        actual_data = data['data']

        # result
        if 'result' not in actual_data:
            raise ValueError("Missing 'result' key in data of S2C_TEMP_CHATROOM_CHAT_SEND_ACK")
        if not isinstance(actual_data['result'], int):
            raise ValueError("The 'result' value must be an integer")
        # chatIdx
        if 'chatIdx' not in actual_data:
            raise ValueError("Missing 'chatIdx' key in data of S2C_TEMP_CHATROOM_CHAT_SEND_ACK")
        if not isinstance(actual_data['chatIdx'], int):
            raise ValueError("The 'chatIdx' value must be an integer")
        # chat
        if 'chat' not in actual_data:
            raise ValueError("Missing 'chat' key in data of S2C_TEMP_CHATROOM_CHAT_SEND_ACK")
        if not isinstance(actual_data['chat'], str):
            raise ValueError("The 'chat' value must be an integer")
        # utcMs
        if 'utcMs' not in actual_data:
            raise ValueError("Missing 'utcMs' key in data of S2C_TEMP_CHATROOM_CHAT_SEND_ACK")
        if not isinstance(actual_data['utcMs'], int):
            raise ValueError("The 'utcMs' value must be an integer")
        
        return cls(
            result = actual_data['result'],
            chat_idx = actual_data['chatIdx'],
            chat = actual_data['chat'],
            utc_ms = actual_data['utcMs']
        )
    
    def to_dict(self):
        return {
            "code": S2C.S2C_TEMP_CHATROOM_CHAT_SEND_ACK.value,
            "data": {
                "result": self.result,
                "chatIdx": self.chat_idx,
                "chat": self.chat,
                "utcMs": self.utc_ms
            }
        }