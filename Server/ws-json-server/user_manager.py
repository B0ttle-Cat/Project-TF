import logging

connected_websockets = set()

class UserManager:
    @staticmethod
    def add_user(websocket):
        logging.info(f"websocket: {websocket.remote_address}, id: {id(websocket)}")
        connected_websockets.add(websocket)
    
    @staticmethod
    def remove_user(websocket):
        logging.info(f"websocket: {websocket.remote_address}, id: {id(websocket)}")
        connected_websockets.remove(websocket)

    @staticmethod
    async def round(except_websocket, raw_data):
        logging.info(f"except_websocket: {except_websocket.remote_address}, id: {id(except_websocket)}")
        for websocket in connected_websockets:
            if websocket == except_websocket:
                continue
            
            try:
                await websocket.send(raw_data)
            except Exception as e:
                logging.error(f"exception occured. message: {e}")