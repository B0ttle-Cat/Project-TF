
from websockets.server import WebSocketServerProtocol

class PacketParser:
    async def parse_packet(raw_data: any, websocket: WebSocketServerProtocol):
        raise NotImplementedError("parse_packet must be implemented by a subclass")
    
    async def parse_C2S_TEMP_CHATROOM_ENTER_REQ(code: int, data: any, websocket: WebSocketServerProtocol):
        raise NotImplementedError("parse_C2S_TEMP_CHATROOM_ENTER_REQ must be implemented by a subclass")
    
    async def parse_C2S_TEMP_CHATROOM_LEAVE_REQ(code: int, data: any, websocket: WebSocketServerProtocol):
        raise NotImplementedError("parse_C2S_TEMP_CHATROOM_LEAVE_REQ must be implemented by a subclass")
    
    async def parse_C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ(code: int, data: any, websocket: WebSocketServerProtocol):
        raise NotImplementedError("parse_C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ must be implemented by a subclass")
    
    async def parse_C2S_TEMP_CHATROOM_CHAT_SEND_REQ(code: int, data: any, websocket: WebSocketServerProtocol):
        raise NotImplementedError("parse_C2S_TEMP_CHATROOM_CHAT_SEND_REQ must be implemented by a subclass")