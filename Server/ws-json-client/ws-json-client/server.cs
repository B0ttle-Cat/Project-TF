using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ws_json_client
{
    public class WebSocketClient
    {
        private ClientWebSocket _webSocket;
        private Uri _serverUri;
        private CancellationTokenSource _cts;
        private Dictionary<int, Action<IPacket>> _callbacks;
        private Action<bool> _connectCallback;
        private Action<bool> _disconnectCallback;

        public WebSocketClient(string serverAddress, int port)
        {
            _serverUri = new Uri($"ws://{serverAddress}:{port}");
            _webSocket = new ClientWebSocket();
            _cts = new CancellationTokenSource();
            _callbacks = new Dictionary<int, Action<IPacket>>();
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _webSocket.ConnectAsync(_serverUri, _cts.Token);
                Console.WriteLine("Connected to server");
                _connectCallback?.Invoke(true);
                await ReceiveMessages();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to server: {e.Message}");
                _connectCallback?.Invoke(false);
            }
        }

        public async Task SendPacketAsync(IPacket packet)
        {
            try
            {
                var json = packet.ToJson().GetRawText();
                var buffer = Encoding.UTF8.GetBytes(json);
                var segment = new ArraySegment<byte>(buffer);
                await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, _cts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending packet: {e.Message}");
            }
        }

        private async Task ReceiveMessages()
        {
            var buffer = new byte[1024 * 4];

            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed connection", _cts.Token);
                        Console.WriteLine("Connection closed by server");
                    }
                    else
                    {
                        var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        HandleIncomingPacket(json);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error receiving message: {e.Message}");
                }
            }
        }

        private void HandleIncomingPacket(string json)
        {
            try
            {
                JsonDocument doc = JsonDocument.Parse(json);
                int code = doc.RootElement.GetProperty("code").GetInt32();

                if (_callbacks.TryGetValue(code, out var callback))
                {
                    IPacket packet = CreatePacketFromJson(code, doc.RootElement);
                    if (packet != null)
                    {
                        callback.Invoke(packet);
                    }
                }
                else
                {
                    Console.WriteLine("No callback registered for packet code: " + code);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error handling incoming packet: {e.Message}");
            }
        }

        private IPacket CreatePacketFromJson(int code, JsonElement json)
        {
            switch ((S2C)code)
            {
                case S2C.S2C_TEMP_CHATROOM_ENTER_ACK:
                    return S2C_TEMP_CHATROOM_ENTER_ACK.FromJson(json);
                case S2C.S2C_TEMP_CHATROOM_ENTER_NTY:
                    return S2C_TEMP_CHATROOM_ENTER_NTY.FromJson(json);
                case S2C.S2C_TEMP_CHATROOM_LEAVE_ACK:
                    return S2C_TEMP_CHATROOM_LEAVE_ACK.FromJson(json);
                case S2C.S2C_TEMP_CHATROOM_LEAVE_NTY:
                    return S2C_TEMP_CHATROOM_LEAVE_NTY.FromJson(json);
                case S2C.S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK:
                    return S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK.FromJson(json);
                case S2C.S2C_TEMP_CHATROOM_CHAT_SEND_ACK:
                    return S2C_TEMP_CHATROOM_CHAT_SEND_ACK.FromJson(json);
                case S2C.S2C_TEMP_CHATROOM_CHAT_SEND_NTY:
                    return S2C_TEMP_CHATROOM_CHAT_SEND_NTY.FromJson(json);
                default:
                    Console.WriteLine("Unknown packet code: " + code);
                    return null;
            }
        }

        public void RegisterCallback(int code, Action<IPacket> callback)
        {
            Console.WriteLine($"code: {code}");
            if (!_callbacks.ContainsKey(code))
            {
                _callbacks.Add(code, callback);
            }
            else
            {
                _callbacks[code] = callback;
            }
        }

        public void RegisterConnectCallback(Action<bool> callback)
        {
            _connectCallback = callback;
        }

        public void RegisterDisconnectCallback(Action<bool> callback)
        {
            _disconnectCallback = callback;
        }

        public async Task DisconnectAsync()
        {
            try
            {
                _cts.Cancel();
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
                Console.WriteLine("Disconnected from server");
                _disconnectCallback?.Invoke(true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error disconnecting: {e.Message}");
                _disconnectCallback?.Invoke(false);
            }
        }
    }
}
