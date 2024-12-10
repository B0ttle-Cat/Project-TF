from enum import Enum

class C2S(Enum):
    C2S_TEMP_CHATROOM_ENTER_REQ = 1000000
    C2S_TEMP_CHATROOM_LEAVE_REQ = 1000001
    C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ = 1000002
    C2S_TEMP_CHATROOM_CHAT_SEND_REQ = 1000003

class S2C(Enum):
    S2C_TEMP_CHATROOM_ENTER_ACK = 2000000
    S2C_TEMP_CHATROOM_ENTER_NTY = 2000001
    S2C_TEMP_CHATROOM_LEAVE_ACK = 2000002
    S2C_TEMP_CHATROOM_LEAVE_NTY = 2000003
    S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK = 2000004
    S2C_TEMP_CHATROOM_CHAT_SEND_ACK = 2000005
    S2C_TEMP_CHATROOM_CHAT_SEND_NTY = 2000006

class Result(Enum):
    SUCCEED = 0
    FAILED_INVALID_FORMAT = 1
    FAILED_INVALID_USERIDX = 2
    FAILED_NOT_IMPLEMENTED_YET = 999