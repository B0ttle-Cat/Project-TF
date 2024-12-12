import logging
import json
from websockets.server import WebSocketServerProtocol
from protocol import C2S
from packet import *
from packet_handler import PacketHandler

class JsonPacketParser:
    def __init__(self):
        super().__init__()
        self.prase_callbacks = {
            C2S.C2S_TEMP_CHATROOM_ENTER_REQ.value: self.parse_C2S_TEMP_CHATROOM_ENTER_REQ,
            C2S.C2S_TEMP_CHATROOM_LEAVE_REQ.value: self.parse_C2S_TEMP_CHATROOM_LEAVE_REQ,
            C2S.C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ.value: self.parse_C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ,
            C2S.C2S_TEMP_CHATROOM_CHAT_SEND_REQ.value: self.parse_C2S_TEMP_CHATROOM_CHAT_SEND_REQ
        }

    async def parse_packet(self, raw_data: any, websocket: WebSocketServerProtocol):
        logging.info(f"called from {websocket.remote_address}")
        try:
            data = json.loads(raw_data)

            if not isinstance(data, dict):
                raise ValueError("Data is not a dictionary")
            if 'code' not in data:
                raise ValueError("Missing 'code' key in data")
            if not isinstance(data['code'], int):
                raise ValueError("The 'code' value must be an integer")
            if 'data' not in data:
                raise ValueError("Missing 'data' key in data")
            if not isinstance(data['data'], dict):
                raise ValueError("The 'data' value must be an object")
            
            # Call the appropriate callback based on the code
            code = data['code']
            if code in self.prase_callbacks:
                await self.prase_callbacks[code](code, data, websocket)
            else:
                logging.error(f"No handler found for code: {code}")
                error_response = {
                    "status": "error",
                    "message": f"No handler found for code: {code}"
                }
                await websocket.send(json.dumps(error_response))

        except json.JSONDecodeError:
            # Handle invalid JSON
            logging.error(f"Invalid JSON received: {raw_data}")
            error_response = {
                "status": "error",
                "message": "Invalid JSON format"
            }
            await websocket.send(json.dumps(error_response))

        except ValueError as e:
            # Handle validation errors
            logging.error(f"Data validation error: {e}")
            error_response = {
                "status": "error",
                "message": str(e)
            }
            await websocket.send(json.dumps(error_response))
            return

    async def parse_C2S_TEMP_CHATROOM_ENTER_REQ(self, code: int, data: any, websocket: WebSocketServerProtocol):
        logging.info(f"code: {code}, data: {data}, addr: {websocket.remote_address}")
        packet = C2S_TEMP_CHATROOM_ENTER_REQ.from_dict(data)
        await PacketHandler.handle_C2S_TEMP_CHATROOM_ENTER_REQ(packet, websocket)
    
    async def parse_C2S_TEMP_CHATROOM_LEAVE_REQ(self, code: int, data: any, websocket: WebSocketServerProtocol):
        logging.info(f"code: {code}, data: {data}, addr: {websocket.remote_address}")
        packet= C2S_TEMP_CHATROOM_LEAVE_REQ.from_dict(data)
        await PacketHandler.handle_C2S_TEMP_CHATROOM_LEAVE_REQ(packet, websocket)
    
    async def parse_C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ(self, code: int, data: any, websocket: WebSocketServerProtocol):
        logging.info(f"code: {code}, data: {data}, addr: {websocket.remote_address}")
        packet= C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ.from_dict(data)
        await PacketHandler.handle_C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ(packet, websocket)
    
    async def parse_C2S_TEMP_CHATROOM_CHAT_SEND_REQ(self, code: int, data: any, websocket: WebSocketServerProtocol):
        logging.info(f"code: {code}, data: {data}, addr: {websocket.remote_address}")
        packet = C2S_TEMP_CHATROOM_CHAT_SEND_REQ.from_dict(data)
        await PacketHandler.handle_C2S_TEMP_CHATROOM_CHAT_SEND_REQ(packet, websocket)
    
    async def parse_C2S_GAMEROOM_ENTER_REQ(self, code: int, data: any, websocket: WebSocketServerProtocol):
        logging.info(f"code: {code}, data: {data}, addr: {websocket.remote_address}")
        packet = C2S_GAMEROOM_ENTER_REQ.from_dict(data)
        await PacketHandler.handle_C2S_GAMEROOM_ENTER_REQ(packet, websocket)