using System;
using System.Windows.Forms;
using System.Text.Json;
using System.Drawing;
using ws_json_client;


public class Program
{
    private static StandardWebSocketClient _client;
    private static Form _mainForm;
    private static TextBox _receivedMessagesTextBox;
    private static TextBox _sendMessageTextBox;
    private static Button _connectButton;
    private static Button _sendButton;
    private static TextBox _hostTextBox;
    private static TextBox _portTextBox;

    [STAThread]
    public static void Main(string[] args)
    {
        // UI 모드 확인
        if (args.Length > 0 && args[0] == "--console")
        {
            // 콘솔 모드
            RunConsoleMode();
        }
        else
        {
            // GUI 모드
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RunGuiMode();
        }
    }

    private static void RunConsoleMode()
    {
        using (var client = new StandardWebSocketClient())
        {
            client.RegisterMessageCallback(message =>
            {
                Console.WriteLine($"received: {message}");
            });

            try
            {
                client.ConnectAsync("localhost", 38201).Wait();

                client.SendMessageAsync(JsonSerializer.Serialize(new
                {
                    type = "greeting",
                    message = "안녕하세요!"
                })).Wait();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
            }
        }
        Console.WriteLine("RunConsoleMode called");
    }

    private static void RunGuiMode()
    {
        _client = new StandardWebSocketClient();

        _mainForm = new Form
        {
            Text = "WebSocket 클라이언트",
            Width = 600,
            Height = 500
        };

        // 호스트 및 포트 입력
        var hostLabel = new Label
        {
            Text = "호스트:",
            Location = new Point(10, 10),
            Width = 100
        };
        _hostTextBox = new TextBox
        {
            Text = "localhost",
            Location = new Point(120, 10),
            Width = 150
        };

        var portLabel = new Label
        {
            Text = "포트:",
            Location = new Point(280, 10),
            Width = 100
        };
        _portTextBox = new TextBox
        {
            Text = "38201",
            Location = new Point(380, 10),
            Width = 100
        };

        // 연결 버튼
        _connectButton = new Button
        {
            Text = "연결",
            Location = new Point(490, 10),
            Width = 80
        };
        _connectButton.Click += ConnectButton_Click;

        // 수신 메시지 박스
        var receivedLabel = new Label
        {
            Text = "수신 메시지:",
            Location = new Point(10, 50),
            Width = 200
        };
        _receivedMessagesTextBox = new TextBox
        {
            Multiline = true,
            Location = new Point(10, 80),
            Width = 560,
            Height = 150,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true
        };

        // 송신 메시지 입력
        var sendLabel = new Label
        {
            Text = "송신 메시지:",
            Location = new Point(10, 240),
            Width = 200
        };
        _sendMessageTextBox = new TextBox
        {
            Multiline = true,
            Location = new Point(10, 270),
            Width = 560,
            Height = 100,
            ScrollBars = ScrollBars.Vertical
        };

        // 전송 버튼
        _sendButton = new Button
        {
            Text = "전송",
            Location = new Point(490, 380),
            Width = 80
        };
        _sendButton.Click += SendButton_Click;

        // 컨트롤 추가
        _mainForm.Controls.AddRange(new Control[]
        {
            hostLabel, _hostTextBox,
            portLabel, _portTextBox,
            _connectButton,
            receivedLabel, _receivedMessagesTextBox,
            sendLabel, _sendMessageTextBox,
            _sendButton
        });

        // 콜백 등록
        _client.RegisterMessageCallback(message =>
        {
            _receivedMessagesTextBox.Invoke((MethodInvoker)delegate
            {
                _receivedMessagesTextBox.AppendText($"recv => {message}" + Environment.NewLine);
            });
        });

        Application.Run(_mainForm);
    }

    private static async void ConnectButton_Click(object sender, EventArgs e)
    {
        try
        {
            await _client.ConnectAsync(_hostTextBox.Text, int.Parse(_portTextBox.Text));
            MessageBox.Show("서버 연결 성공!", "연결", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"연결 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static async void SendButton_Click(object sender, EventArgs e)
    {
        try
        {
            string body = JsonSerializer.Serialize(new
            {
                code = C2S.C2S_TEMP_CHATROOM_CHAT_SEND_REQ,
                data = new
                {
                    userIdx = 0,
                    chat = _sendMessageTextBox.Text
                }
            });
            await _client.SendMessageAsync(body);
            _receivedMessagesTextBox.AppendText($"send => {_sendMessageTextBox.Text}" + Environment.NewLine);
            _sendMessageTextBox.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"전송 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}