import logging
import json
from websockets.server import WebSocketServerProtocol
from protocol import C2S, S2C, Result
from packet import C2S_TEMP_CHATROOM_CHAT_SEND_REQ, S2C_TEMP_CHATROOM_CHAT_SEND_ACK

class PacketHandler:
    @staticmethod
    async def handle_C2S_TEMP_CHATROOM_CHAT_SEND_REQ(packet: C2S_TEMP_CHATROOM_CHAT_SEND_REQ, websocket: WebSocketServerProtocol):
        logging.info(f'packet: {packet}, addr:{websocket.remote_address}')
        ackPacket = S2C_TEMP_CHATROOM_CHAT_SEND_ACK(
            result = Result.SUCCEED.value,
            chat_idx = 0,
            chat = packet.chat,
            utc_ms = 0
        )

        await websocket.send(json.dumps(ackPacket.to_dict(), ensure_ascii=False))