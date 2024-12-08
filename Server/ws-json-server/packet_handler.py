import logging
import json
from websockets.server import WebSocketServerProtocol
from protocol import C2S, S2C, Result
from packet import *
from user_manager import UserManager
import datetime

curr_chat_idx = 0
curr_user_idx = 0
users = {}

class User:
    def __init__(self, user_idx, nickname, websocket):
        self.user_idx = user_idx
        self.nickname = nickname
        self.websocket = websocket

class PacketHandler:

    @staticmethod
    async def broadcast(except_user_idx, packet):
        global users
        for user_idx in users.keys():
            if user_idx == except_user_idx:
                continue
            
            logging.info(f'sending to {users[user_idx].nickname}({user_idx})')
            await users[user_idx].websocket.send(json.dumps(packet.to_dict(), ensure_ascii=False))

    @staticmethod
    async def handle_C2S_TEMP_CHATROOM_ENTER_REQ(packet: C2S_TEMP_CHATROOM_ENTER_REQ, websocket: WebSocketServerProtocol):
        logging.info(f'packet: {packet.to_dict()}, addr:{websocket.remote_address}')

        global users
        global curr_user_idx

        user_idx = curr_user_idx
        curr_user_idx += 1

        users[user_idx] = User(user_idx, packet.nickname, websocket)
        logging.info(f'new user added. user_idx: {user_idx}')

        ackPacket = S2C_TEMP_CHATROOM_ENTER_ACK(
            result = Result.SUCCEED.value,
            user_idx = user_idx,
            nickname = packet.nickname
        )

        await websocket.send(json.dumps(ackPacket.to_dict(), ensure_ascii=False))

        ntyPacket = S2C_TEMP_CHATROOM_ENTER_NTY(
            user_idx = user_idx,
            nickname = packet.nickname
        )

        await PacketHandler.broadcast(user_idx, ntyPacket)


    @staticmethod
    async def handle_C2S_TEMP_CHATROOM_LEAVE_REQ(packet: C2S_TEMP_CHATROOM_LEAVE_REQ, websocket: WebSocketServerProtocol):
        logging.info(f'packet: {packet.to_dict()}, addr:{websocket.remote_address}')
        
        global users
        del users[packet.user_idx]

        ackPacket = S2C_TEMP_CHATROOM_LEAVE_ACK(
            result = Result.SUCCEED.value
        )

        await websocket.send(json.dumps(ackPacket.to_dict(), ensure_ascii=False))

        ntyPacket = S2C_TEMP_CHATROOM_LEAVE_NTY(packet.user_idx)

        await PacketHandler.broadcast(packet.user_idx, ntyPacket)


    @staticmethod
    async def handle_C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ(packet: C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ, websocket: WebSocketServerProtocol):
        logging.info(f'packet: {packet.to_dict()}, addr:{websocket.remote_address}')

        global users
        user_list = []
        for user_idx in users.keys():
            user_list.append({'userIdx': user_idx, 'nickname': users[user_idx].nickname})

        ackPacket = S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK(
            result = Result.FAILED_NOT_IMPLEMENTED_YET.value,
            user_list = user_list
        )

        await websocket.send(json.dumps(ackPacket.to_dict(), ensure_ascii=False))

    @staticmethod
    async def handle_C2S_TEMP_CHATROOM_CHAT_SEND_REQ(packet: C2S_TEMP_CHATROOM_CHAT_SEND_REQ, websocket: WebSocketServerProtocol):
        global curr_chat_idx
        logging.info(f'packet: {packet.to_dict()}, addr:{websocket.remote_address}')
        
        curr_utc_ms = int(datetime.datetime.utcnow().timestamp() * 1000)
        
        ackPacket = S2C_TEMP_CHATROOM_CHAT_SEND_ACK(
            result = Result.SUCCEED.value,
            chat_idx = curr_chat_idx,
            chat = packet.chat,
            utc_ms = curr_utc_ms
        )

        await websocket.send(json.dumps(ackPacket.to_dict(), ensure_ascii=False))

        ntyPacket = S2C_TEMP_CHATROOM_CHAT_SEND_NTY(
            user_idx = packet.user_idx,
            chat_idx = curr_chat_idx,
            chat = packet.chat,
            utc_ms = curr_utc_ms
        )

        await PacketHandler.broadcast(packet.user_idx, ntyPacket)

        curr_chat_idx += 1
    

    @staticmethod
    async def on_close(websocket):
        logging.info(f'addr:{websocket.remote_address}')
        global users
        
        del_user_idx = 0
        for user_idx in users.keys():
            if users[user_idx].websocket == websocket:
                del users[user_idx]
                del_user_idx = user_idx
                break
        
        ntyPacket = S2C_TEMP_CHATROOM_LEAVE_NTY(del_user_idx)

        await PacketHandler.broadcast(del_user_idx, ntyPacket)
        