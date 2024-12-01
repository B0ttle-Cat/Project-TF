import asyncio
import websockets
import json
import logging
import argparse
from json_event_receiver import JsonEventReceiver

# Configure logging
logging.basicConfig(level=logging.INFO, 
                    format='%(asctime)s - %(levelname)s - %(module)s::%(funcName)s - %(message)s')

event_receiver = JsonEventReceiver()

async def handle_connection(websocket, path: str = None):
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
                websocket.close()
            
            except Exception as e:
                # Handle any other processing errors
                logging.error(f"Error processing message: {e}")
                websocket.close()
    
    except websockets.exceptions.ConnectionClosed:
        logging.info("WebSocket connection closed")
        await event_receiver.on_close(websocket)
    
    except Exception as e:
        logging.error(f"Connection handler error: {e}")

async def start_server(port: int):
    """
    Start the WebSocket server.
    """
    # Create and start the WebSocket server
    server = await websockets.serve(
        handle_connection, 
        "0.0.0.0", 
        port
    )
    
    # Get the actual port used
    port = server.sockets[0].getsockname()[1]
    logging.info(f"WebSocket server started on ws://localhost:{port}")
    
    # Keep the server running
    await server.wait_closed()

# Main entry point
if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="WebSocket server")
    parser.add_argument("--port", type=int, required=True, help="Port number to start the WebSocket server")
    args = parser.parse_args()

    try:
        # Run the server using asyncio
        asyncio.run(start_server(args.port))
    except KeyboardInterrupt:
        logging.info("Server stopped by user")
    except Exception as e:
        logging.error(f"Server error: {e}")