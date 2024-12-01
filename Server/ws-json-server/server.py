import asyncio
import websockets
import json
import logging
from json_event_receiver import JsonEventReceiver

# Configure logging
logging.basicConfig(level=logging.INFO, 
                    format='%(asctime)s - %(levelname)s - %(module)s::%(funcName)s - %(message)s')

event_receiver = JsonEventReceiver()

async def handle_connection(websocket: websockets.server.ServerConnection, path: str = None):
    """
    Handle incoming WebSocket connections and process JSON data.
    
    :param websocket: WebSocket connection object
    :param path: Connection path (optional)
    """
    try:
        logging.info("connection open")
        event_receiver.on_accept(websocket)
        
        # Receive data from the client
        async for raw_data in websocket:
            try:
                await event_receiver.on_receive(websocket, raw_data)
                
            except json.JSONDecodeError:
                # Handle invalid JSON
                logging.error(f"Invalid JSON received: {raw_data}")
                error_response = {
                    "status": "error",
                    "message": "Invalid JSON format"
                }
                await websocket.send(json.dumps(error_response))
            
            except Exception as e:
                # Handle any other processing errors
                logging.error(f"Error processing message: {e}")
                error_response = {
                    "status": "error",
                    "message": str(e)
                }
                await websocket.send(json.dumps(error_response))
    
    except websockets.exceptions.ConnectionClosed:
        logging.info("WebSocket connection closed")
        event_receiver.on_close(websocket)

    except Exception as e:
        logging.error(f"Connection handler error: {e}")

async def start_server(port: int):
    """
    Start the WebSocket server.
    """
    # Create and start the WebSocket server
    server = await websockets.serve(
        handle_connection, 
        "localhost", 
        port
    )
    
    # Get the actual port used
    port = server.sockets[0].getsockname()[1]
    logging.info(f"WebSocket server started on ws://localhost:{port}")
    
    # Keep the server running
    await server.wait_closed()

# Main entry point
if __name__ == "__main__":
    try:
        # Run the server using asyncio
        asyncio.run(start_server(38201))
    except KeyboardInterrupt:
        logging.info("Server stopped by user")
    except Exception as e:
        logging.error(f"Server error: {e}")