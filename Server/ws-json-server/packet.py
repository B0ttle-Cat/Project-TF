from protocol import C2S, S2C, Result

# C2S
class C2S_TEMP_CHATROOM_ENTER_REQ:
    code = C2S.C2S_TEMP_CHATROOM_ENTER_REQ.value

    def __init__(self, nickname: str):
        self.nickname = nickname

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in C2S_TEMP_CHATROOM_ENTER_REQ")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")
        
        actual_data = data['data']

        if 'nickname' not in actual_data:
            raise ValueError("Missing 'nickname' key in data")
        if not isinstance(actual_data['nickname'], str):
            raise ValueError("The 'nickname' value must be a string")

        return cls(
            nickname=actual_data['nickname']
        )

    def to_dict(self):
        return {
            "code": C2S.C2S_TEMP_CHATROOM_ENTER_REQ.value,
            "data": {
                "nickname": self.nickname
            }
        }


class C2S_TEMP_CHATROOM_LEAVE_REQ:
    code = C2S.C2S_TEMP_CHATROOM_LEAVE_REQ.value

    def __init__(self, user_idx: int):
        self.user_idx = user_idx

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in C2S_TEMP_CHATROOM_LEAVE_REQ")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")
        
        actual_data = data['data']

        if 'userIdx' not in actual_data:
            raise ValueError("Missing 'userIdx' key in data")
        if not isinstance(actual_data['userIdx'], int):
            raise ValueError("The 'userIdx' value must be an integer")

        return cls(
            user_idx=actual_data['userIdx']
        )

    def to_dict(self):
        return {
            "code": C2S.C2S_TEMP_CHATROOM_LEAVE_REQ.value,
            "data": {
                "userIdx": self.user_idx
            }
        }


class C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ:
    code = C2S.C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ.value

    def __init__(self, user_idx: int):
        self.user_idx = user_idx

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")
        
        actual_data = data['data']

        if 'userIdx' not in actual_data:
            raise ValueError("Missing 'userIdx' key in data")
        if not isinstance(actual_data['userIdx'], int):
            raise ValueError("The 'userIdx' value must be an integer")

        return cls(
            user_idx=actual_data['userIdx']
        )

    def to_dict(self):
        return {
            "code": C2S.C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ.value,
            "data": {
                "userIdx": self.user_idx
            }
        }


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
            user_idx=actual_data['userIdx'],
            chat=actual_data['chat']
        )

    def to_dict(self):
        return {
            "code": C2S.C2S_TEMP_CHATROOM_CHAT_SEND_REQ.value,
            "data": {
                "userIdx": self.user_idx,
                "chat": self.chat
            }
        }

class C2S_GAMEROOM_ENTER_REQ:
    code = C2S.C2S_GAMEROOM_ENTER_REQ.value

    def __init__(self, user_idx: int, nickname: str, map_hash: str):
        self.user_idx = user_idx
        self.nickname = nickname
        self.map_hash = map_hash

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in C2S_GAMEROOM_ENTER_REQ")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")

        actual_data = data['data']

        if 'userIdx' not in actual_data:
            raise ValueError("Missing 'userIdx' key in data")
        if not isinstance(actual_data['userIdx'], int):
            raise ValueError("The 'userIdx' value must be an integer")

        if 'nickname' not in actual_data:
            raise ValueError("Missing 'nickname' key in data")
        if not isinstance(actual_data['nickname'], str):
            raise ValueError("The 'nickname' value must be a string")

        if 'mapHash' not in actual_data:
            raise ValueError("Missing 'mapHash' key in data")
        if not isinstance(actual_data['mapHash'], str):
            raise ValueError("The 'mapHash' value must be a string")

        return cls(
            user_idx=actual_data['userIdx'],
            nickname=actual_data['nickname'],
            map_hash=actual_data['mapHash']
        )

    def to_dict(self):
        return {
            "code": C2S.C2S_GAMEROOM_ENTER_REQ.value,
            "data": {
                "userIdx": self.user_idx,
                "nickname": self.nickname,
                "mapHash": self.map_hash
            }
        }

# S2C
class S2C_TEMP_CHATROOM_ENTER_ACK:
    code = S2C.S2C_TEMP_CHATROOM_ENTER_ACK.value

    def __init__(self, result: int, user_idx: int, nickname: str):
        self.result = result
        self.user_idx = user_idx
        self.nickname = nickname

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in S2C_TEMP_CHATROOM_ENTER_ACK")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")
        
        actual_data = data['data']

        if 'result' not in actual_data:
            raise ValueError("Missing 'result' key in data")
        if not isinstance(actual_data['result'], int):
            raise ValueError("The 'result' value must be a integer")
        
        if 'userIdx' not in actual_data:
            raise ValueError("Missing 'userIdx' key in data")
        if not isinstance(actual_data['userIdx'], int):
            raise ValueError("The 'userIdx' value must be an integer")
        
        if 'nickname' not in actual_data:
            raise ValueError("Missing 'nickname' key in data")
        if not isinstance(actual_data['nickname'], str):
            raise ValueError("The 'nickname' value must be a string")

        return cls(
            result=actual_data['result'],
            user_idx=actual_data['userIdx'],
            nickname=actual_data['nickname']
        )

    def to_dict(self):
        return {
            "code": S2C.S2C_TEMP_CHATROOM_ENTER_ACK.value,
            "data": {
                "result": self.result,
                "userIdx": self.user_idx,
                "nickname": self.nickname
            }
        }


class S2C_TEMP_CHATROOM_ENTER_NTY:
    code = S2C.S2C_TEMP_CHATROOM_ENTER_NTY.value

    def __init__(self, user_idx: int, nickname: str):
        self.user_idx = user_idx
        self.nickname = nickname

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in S2C_TEMP_CHATROOM_ENTER_NTY")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")
        
        actual_data = data['data']

        if 'userIdx' not in actual_data:
            raise ValueError("Missing 'userIdx' key in data")
        if not isinstance(actual_data['userIdx'], int):
            raise ValueError("The 'userIdx' value must be an integer")
        
        if 'nickname' not in actual_data:
            raise ValueError("Missing 'nickname' key in data")
        if not isinstance(actual_data['nickname'], str):
            raise ValueError("The 'nickname' value must be a string")

        return cls(
            user_idx=actual_data['userIdx'],
            nickname=actual_data['nickname']
        )

    def to_dict(self):
        return {
            "code": S2C.S2C_TEMP_CHATROOM_ENTER_NTY.value,
            "data": {
                "userIdx": self.user_idx,
                "nickname": self.nickname
            }
        }


class S2C_TEMP_CHATROOM_LEAVE_ACK:
    code = S2C.S2C_TEMP_CHATROOM_LEAVE_ACK.value

    def __init__(self, result: int):
        self.result = result

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in S2C_TEMP_CHATROOM_LEAVE_ACK")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")
        
        actual_data = data['data']

        if 'result' not in actual_data:
            raise ValueError("Missing 'result' key in data")
        if not isinstance(actual_data['result'], int):
            raise ValueError("The 'result' value must be a integer")

        return cls(
            result=actual_data['result']
        )

    def to_dict(self):
        return {
            "code": S2C.S2C_TEMP_CHATROOM_LEAVE_ACK.value,
            "data": {
                "result": self.result
            }
        }

class S2C_TEMP_CHATROOM_LEAVE_NTY:
    code = S2C.S2C_TEMP_CHATROOM_LEAVE_NTY.value

    def __init__(self, user_idx: int):
        self.user_idx = user_idx

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in S2C_TEMP_CHATROOM_LEAVE_NTY")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")
        
        actual_data = data['data']

        if 'userIdx' not in actual_data:
            raise ValueError("Missing 'userIdx' key in data")
        if not isinstance(actual_data['userIdx'], int):
            raise ValueError("The 'userIdx' value must be an integer")

        return cls(
            user_idx=actual_data['userIdx']
        )

    def to_dict(self):
        return {
            "code": S2C.S2C_TEMP_CHATROOM_LEAVE_NTY.value,
            "data": {
                "userIdx": self.user_idx
            }
        }


class S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK:
    code = S2C.S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK.value

    def __init__(self, result: int, user_list: list):
        self.result = result
        self.user_list = user_list

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")
        
        actual_data = data['data']

        if 'result' not in actual_data:
            raise ValueError("Missing 'result' key in data")
        if not isinstance(actual_data['result'], InterruptedError):
            raise ValueError("The 'result' value must be a string")
        
        if 'userList' not in actual_data:
            raise ValueError("Missing 'userList' key in data")
        if not isinstance(actual_data['userList'], list):
            raise ValueError("The 'userList' value must be a list")

        user_list = []
        for user in actual_data['userList']:
            if 'userIdx' not in user or 'nickname' not in user:
                raise ValueError("Each user in 'userList' must contain 'userIdx' and 'nickname' keys")
            if not isinstance(user['userIdx'], int):
                raise ValueError("The 'userIdx' value must be an integer")
            if not isinstance(user['nickname'], str):
                raise ValueError("The 'nickname' value must be a string")
            
            user_list.append({
                "userIdx": user['userIdx'],
                "nickname": user['nickname']
            })

        return cls(
            result=actual_data['result'],
            user_list=user_list
        )

    def to_dict(self):
        return {
            "code": S2C.S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK.value,
            "data": {
                "result": self.result,
                "userList": self.user_list
            }
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
            result=actual_data['result'],
            chat_idx=actual_data['chatIdx'],
            chat=actual_data['chat'],
            utc_ms=actual_data['utcMs']
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


class S2C_TEMP_CHATROOM_CHAT_SEND_NTY:
    code = S2C.S2C_TEMP_CHATROOM_CHAT_SEND_NTY.value

    def __init__(self, user_idx: int, chat_idx: int, chat: str, utc_ms: int):
        self.user_idx = user_idx
        self.chat_idx = chat_idx
        self.chat = chat
        self.utc_ms = utc_ms

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in S2C_TEMP_CHATROOM_CHAT_SEND_NTY")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")
        
        actual_data = data['data']

        if 'userIdx' not in actual_data:
            raise ValueError("Missing 'userIdx' key in data")
        if not isinstance(actual_data['userIdx'], int):
            raise ValueError("The 'userIdx' value must be an integer")
        
        if 'chatIdx' not in actual_data:
            raise ValueError("Missing 'chatIdx' key in data")
        if not isinstance(actual_data['chatIdx'], int):
            raise ValueError("The 'chatIdx' value must be an integer")
        
        if 'chat' not in actual_data:
            raise ValueError("Missing 'chat' key in data")
        if not isinstance(actual_data['chat'], str):
            raise ValueError("The 'chat' value must be a string")
        
        if 'utcMs' not in actual_data:
            raise ValueError("Missing 'utcMs' key in data")
        if not isinstance(actual_data['utcMs'], int):
            raise ValueError("The 'utcMs' value must be an integer")

        return cls(
            user_idx=actual_data['userIdx'],
            chat_idx=actual_data['chatIdx'],
            chat=actual_data['chat'],
            utc_ms=actual_data['utcMs']
        )

    def to_dict(self):
        return {
            "code": S2C.S2C_TEMP_CHATROOM_CHAT_SEND_NTY.value,
            "data": {
                "userIdx": self.user_idx,
                "chatIdx": self.chat_idx,
                "chat": self.chat,
                "utcMs": self.utc_ms
            }
        }

class S2C_GAMEROOM_ENTER_ACK:
    code = S2C.S2C_GAMEROOM_ENTER_ACK.value

    def __init__(self, result: int, map_size: dict, nodes: list, variant_datas: list):
        self.result = result
        self.map_size = map_size
        self.nodes = nodes
        self.variant_datas = variant_datas

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in S2C_GAMEROOM_ENTER_ACK")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")

        actual_data = data['data']

        if 'result' not in actual_data:
            raise ValueError("Missing 'result' key in data")
        if not isinstance(actual_data['result'], str):
            raise ValueError("The 'result' value must be a string")

        if 'mapSize' not in actual_data:
            raise ValueError("Missing 'mapSize' key in data")
        if not isinstance(actual_data['mapSize'], dict):
            raise ValueError("The 'mapSize' value must be a dictionary")

        if 'nodes' not in actual_data:
            raise ValueError("Missing 'nodes' key in data")
        if not isinstance(actual_data['nodes'], list):
            raise ValueError("The 'nodes' value must be a list")

        if 'variantDatas' not in actual_data:
            raise ValueError("Missing 'variantDatas' key in data")
        if not isinstance(actual_data['variantDatas'], list):
            raise ValueError("The 'variantDatas' value must be a list")

        return cls(
            result=actual_data['result'],
            map_size=actual_data['mapSize'],
            nodes=actual_data['nodes'],
            variant_datas=actual_data['variantDatas']
        )

    def to_dict(self):
        return {
            "code": S2C.S2C_GAMEROOM_ENTER_ACK.value,
            "data": {
                "result": self.result,
                "mapSize": self.map_size,
                "nodes": self.nodes,
                "variantDatas": self.variant_datas
            }
        }

class S2C_GAMEROOM_ENTER_NTY:
    code = S2C.S2C_GAMEROOM_ENTER_NTY.value

    def __init__(self, user_idx: int, nickname: str):
        self.user_idx = user_idx
        self.nickname = nickname

    @classmethod
    def from_dict(cls, data: dict):
        if 'data' not in data:
            raise ValueError("Missing 'data' key in S2C_GAMEROOM_ENTER_NTY")
        if not isinstance(data['data'], dict):
            raise ValueError("The 'data' field must be a dictionary")

        actual_data = data['data']

        if 'userIdx' not in actual_data:
            raise ValueError("Missing 'userIdx' key in data")
        if not isinstance(actual_data['userIdx'], int):
            raise ValueError("The 'userIdx' value must be an integer")

        if 'nickname' not in actual_data:
            raise ValueError("Missing 'nickname' key in data")
        if not isinstance(actual_data['nickname'], str):
            raise ValueError("The 'nickname' value must be a string")

        return cls(
            user_idx=actual_data['userIdx'],
            nickname=actual_data['nickname']
        )

    def to_dict(self):
        return {
            "code": S2C.S2C_GAMEROOM_ENTER_NTY.value,
            "data": {
                "userIdx": self.user_idx,
                "nickname": self.nickname
            }
        }
