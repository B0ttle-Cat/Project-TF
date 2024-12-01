import logging

from json_packet_parser import JsonPacketParser
from websockets.server import WebSocketServerProtocol
from event_receiver import EventReceiver

class JsonEventReceiver(EventReceiver):

    def __init__(self):
        logging.info("hooray! JsonEventReceiver online.")
        self.packet_parser = JsonPacketParser()

    def on_accept(self, websocket: WebSocketServerProtocol):
        logging.info(f"addr: {websocket.remote_address}")

    def on_close(self, websocket: WebSocketServerProtocol):
        logging.info(f"addr: {websocket.remote_address}")

    async def on_receive(self, websocket: WebSocketServerProtocol, raw_data):
        logging.info(f"addr: {websocket.remote_address}")
        await self.packet_parser.parse_packet(raw_data, websocket)