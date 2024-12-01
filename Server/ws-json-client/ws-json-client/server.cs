using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Sockets;

/// <summary>
/// WebSocket 클라이언트의 기본 인터페이스
/// </summary>
public interface IWebSocketClient : IDisposable
{
    /// <summary>
    /// 서버와 WebSocket 연결을 수행합니다.
    /// </summary>
    /// <param name="host">서버 호스트 (IP 또는 도메인)</param>
    /// <param name="port">서버 포트 번호</param>
    /// <returns>연결 작업</returns>
    Task ConnectAsync(string host, int port);

    /// <summary>
    /// JSON 형식의 메시지를 서버로 전송합니다.
    /// </summary>
    /// <param name="jsonMessage">JSON 포맷의 문자열 메시지</param>
    /// <returns>메시지 전송 작업</returns>
    Task SendMessageAsync(string jsonMessage);

    /// <summary>
    /// 서버에서 수신한 메시지 처리를 위한 콜백 함수를 등록합니다.
    /// </summary>
    /// <param name="callback">JSON 메시지를 처리할 콜백 함수</param>
    void RegisterMessageCallback(Action<string> callback);

    /// <summary>
    /// 현재 연결된 서버와의 WebSocket 연결을 종료합니다.
    /// </summary>
    /// <returns>연결 종료 작업</returns>
    Task DisconnectAsync();
}

/// <summary>
/// WebSocket 클라이언트의 표준 구현
/// </summary>
public class StandardWebSocketClient : IWebSocketClient
{
    private ClientWebSocket _client;
    private CancellationTokenSource _cancellationTokenSource;
    private Action<string> _messageCallback;
    private Task _receiveTask;

    public StandardWebSocketClient()
    {
        _client = new ClientWebSocket();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task ConnectAsync(string host, int port)
    {
        // 기존 연결 종료
        await DisconnectAsync();

        // 새 연결 준비
        _client = new ClientWebSocket();
        _cancellationTokenSource = new CancellationTokenSource();

        // URI 생성
        Uri serverUri = new Uri($"ws://{host}:{port}");
        await _client.ConnectAsync(serverUri, _cancellationTokenSource.Token);

        // 메시지 수신 작업 시작
        _receiveTask = ReceiveMessagesAsync();
    }

    public async Task SendMessageAsync(string jsonMessage)
    {
        if (_client.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket not opened.");
        }

        byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
        await _client.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            _cancellationTokenSource.Token
        );
    }

    public void RegisterMessageCallback(Action<string> callback)
    {
        _messageCallback = callback;
    }

    private async Task ReceiveMessagesAsync()
    {
        byte[] buffer = new byte[1024 * 4]; // 4KB 버퍼

        try
        {
            while (_client.State == WebSocketState.Open)
            {
                var result = await _client.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    _cancellationTokenSource.Token
                );

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _client.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Connection closed",
                        _cancellationTokenSource.Token
                    );
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _messageCallback?.Invoke(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ReceiveMessagesAsync. exception occured. message: {ex.Message}");
        }
    }

    public async Task DisconnectAsync()
    {
        if (_client.State == WebSocketState.Open)
        {
            Console.WriteLine("DisconnectAsync. _client opened.");
            await _client.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Disconnecting",
                _cancellationTokenSource.Token
            );
            _cancellationTokenSource.Cancel();
        }
        else
        {
            Console.WriteLine("DisconnectAsync. _client not opened");
        }
        _client.Dispose();
    }

    public void Dispose()
    {
        try
        {
            DisconnectAsync().Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dispose. exception occured. message: {ex.Message}");
        }
        finally
        {
            _client = null;
            _cancellationTokenSource = null;
        }
    }
}
