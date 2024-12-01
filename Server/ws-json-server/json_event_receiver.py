import logging

from json_packet_parser import JsonPacketParser
from websockets.server import WebSocketServerProtocol
from event_receiver import EventReceiver
from user_manager import UserManager
from packet_handler import PacketHandler

class JsonEventReceiver(EventReceiver):
    def __init__(self):
        logging.info("hooray! JsonEventReceiver online.")
        self.packet_parser = JsonPacketParser()

    def on_accept(self, websocket: WebSocketServerProtocol):
        logging.info(f"addr: {websocket.remote_address}, id: {id(websocket)}")
        UserManager.add_user(websocket)

    async def on_close(self, websocket: WebSocketServerProtocol):
        logging.info(f"addr: {websocket.remote_address}")
        UserManager.remove_user(websocket)
        await PacketHandler.on_close(websocket)

    async def on_receive(self, websocket: WebSocketServerProtocol, raw_data):
        logging.info(f"addr: {websocket.remote_address}")
        await self.packet_parser.parse_packet(raw_data, websocket)