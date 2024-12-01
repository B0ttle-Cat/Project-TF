using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.Json;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ws_json_client
{
    public class Program
    {
        private static WebSocketClient _client;
        private static Form _mainForm;
        private static TextBox _receivedMessagesTextBox;
        private static TextBox _sendMessageTextBox;
        private static Button _connectButton;
        private static Button _sendButton;
        private static TextBox _hostTextBox;
        private static TextBox _portTextBox;
        private static TextBox _nicknameTextBox;

        private static int _myUserIdx;
        private static Dictionary<int, string> _userDictionary;

        [STAThread]
        public static void Main(string[] args)
        {
            RunGuiMode();
        }

        private static void RunGuiMode()
        {
            _client = new WebSocketClient("localhost", 38201);

            _mainForm = new Form
            {
                Text = "WebSocket 클라이언트",
                Width = 600,
                Height = 550
            };

            // Host and Port inputs
            var hostLabel = new Label
            {
                Text = "호스트:",
                Location = new Point(10, 10),
                Width = 50
            };
            _hostTextBox = new TextBox
            {
                Text = "localhost",
                Location = new Point(70, 10),
                Width = 150
            };

            var portLabel = new Label
            {
                Text = "포트:",
                Location = new Point(230, 10),
                Width = 50
            };
            _portTextBox = new TextBox
            {
                Text = "38201",
                Location = new Point(290, 10),
                Width = 100
            };

            // Nickname input
            var nicknameLabel = new Label
            {
                Text = "닉네임:",
                Location = new Point(10, 40),
                Width = 50
            };
            _nicknameTextBox = new TextBox
            {
                Text = "",
                Location = new Point(70, 40),
                Width = 320
            };

            // Connect button
            _connectButton = new Button
            {
                Text = "연결",
                Location = new Point(490, 10),
                Width = 80
            };
            _connectButton.Click += ConnectButton_Click;

            // Received messages box
            var receivedLabel = new Label
            {
                Text = "수신 메시지:",
                Location = new Point(10, 80),
                Width = 200
            };
            _receivedMessagesTextBox = new TextBox
            {
                Multiline = true,
                Location = new Point(10, 110),
                Width = 560,
                Height = 150,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };

            // Send message input
            var sendLabel = new Label
            {
                Text = "송신 메시지:",
                Location = new Point(10, 270),
                Width = 200
            };
            _sendMessageTextBox = new TextBox
            {
                Multiline = true,
                Location = new Point(10, 300),
                Width = 560,
                Height = 100,
                ScrollBars = ScrollBars.Vertical
            };

            // Send button
            _sendButton = new Button
            {
                Text = "전송",
                Location = new Point(490, 410),
                Width = 80
            };
            _sendButton.Click += SendButton_Click;

            // Add controls to form
            _mainForm.Controls.AddRange(new Control[]
            {
            hostLabel, _hostTextBox,
            portLabel, _portTextBox,
            nicknameLabel, _nicknameTextBox,
            _connectButton,
            receivedLabel, _receivedMessagesTextBox,
            sendLabel, _sendMessageTextBox,
            _sendButton
            });

            Application.Run(_mainForm);
        }

        private static void InitCallback(WebSocketClient client)
        {
            // Register event callback
            client.RegisterConnectCallback(isConnectSuccess =>
            {
                _receivedMessagesTextBox.Invoke((MethodInvoker)async delegate
                {
                    string message = isConnectSuccess ? "= 서버에 연결 되었습니다 =" : "= 서버 연결에 실패 했습니다, 다시 시도 해주세요 =";
                    _receivedMessagesTextBox.AppendText(message + Environment.NewLine);

                    C2S_TEMP_CHATROOM_ENTER_REQ reqPacket = new C2S_TEMP_CHATROOM_ENTER_REQ(_nicknameTextBox.Text);
                    await _client.SendPacketAsync(reqPacket);
                });
            });
            client.RegisterDisconnectCallback(isDisconnectSuccess =>
            {
                _receivedMessagesTextBox.Invoke((MethodInvoker)delegate
                {
                    string message = isDisconnectSuccess ? "= 서버 연결이 종료 되었습니다 =" : "= 서버 연결 종료에 실패 했습니다 =";
                    _receivedMessagesTextBox.AppendText(message + Environment.NewLine);
                });
            });

            // Register packet callback
            client.RegisterCallback((int)S2C.S2C_TEMP_CHATROOM_ENTER_ACK, packet =>
            {
                _receivedMessagesTextBox.Invoke((MethodInvoker)async delegate
                {
                    var enterAck = packet as S2C_TEMP_CHATROOM_ENTER_ACK;
                    _receivedMessagesTextBox.AppendText($"= 채팅에 참여 하였습니다 (nickname: {enterAck.Nickname}, id: {enterAck.UserIdx}) =" + Environment.NewLine);

                    _myUserIdx = enterAck.UserIdx;

                    C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ reqPacket = new C2S_TEMP_CHATROOM_SNAPSHOT_GET_REQ(enterAck.UserIdx);
                    await _client.SendPacketAsync(reqPacket);
                });
            });
            client.RegisterCallback((int)S2C.S2C_TEMP_CHATROOM_ENTER_NTY, packet =>
            {
                _receivedMessagesTextBox.Invoke((MethodInvoker)delegate
                {
                    var enterNty = packet as S2C_TEMP_CHATROOM_ENTER_NTY;
                    _receivedMessagesTextBox.AppendText($"= 유저 {enterNty.Nickname}({enterNty.UserIdx}) 가 입장 했습니다. =" + Environment.NewLine);

                    _userDictionary.Add(enterNty.UserIdx, enterNty.Nickname);
                });
            });
            client.RegisterCallback((int)S2C.S2C_TEMP_CHATROOM_LEAVE_ACK, packet =>
            {
                _receivedMessagesTextBox.Invoke((MethodInvoker)async delegate
                {
                    var leaveAck = packet as S2C_TEMP_CHATROOM_LEAVE_ACK;
                    _receivedMessagesTextBox.AppendText($"= 채팅에서 퇴장 하였습니다 =" + Environment.NewLine);

                    await _client.DisconnectAsync();
                });
            });
            client.RegisterCallback((int)S2C.S2C_TEMP_CHATROOM_LEAVE_NTY, packet =>
            {
                _receivedMessagesTextBox.Invoke((MethodInvoker)delegate
                {
                    var leaveNty = packet as S2C_TEMP_CHATROOM_LEAVE_NTY;
                    string nickname = _userDictionary[leaveNty.UserIdx];
                    _receivedMessagesTextBox.AppendText($"= 유저 {nickname}({leaveNty.UserIdx}) 가 퇴장 했습니다. =" + Environment.NewLine);

                    _userDictionary.Remove(leaveNty.UserIdx);
                });
            });
            client.RegisterCallback((int)S2C.S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK, packet =>
            {
                _receivedMessagesTextBox.Invoke((MethodInvoker)async delegate
                {
                    var snapshotAck = packet as S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK;

                    foreach (var user in snapshotAck.UserList)
                    {
                        _userDictionary.Add(user.UserIdx, user.Nickname);
                        Console.WriteLine($"userIdx: {user.UserIdx}, nickname: {user.Nickname}");
                    }
                });
            });
            client.RegisterCallback((int)S2C.S2C_TEMP_CHATROOM_CHAT_SEND_ACK, packet =>
            {
                _receivedMessagesTextBox.Invoke((MethodInvoker)delegate
                {
                    var chatSendAck = packet as S2C_TEMP_CHATROOM_CHAT_SEND_ACK;
                    string nickname = _userDictionary[_myUserIdx];
                    _receivedMessagesTextBox.AppendText($"{nickname}(나) => {chatSendAck.Chat}" + Environment.NewLine);
                });
            });
            client.RegisterCallback((int)S2C.S2C_TEMP_CHATROOM_CHAT_SEND_NTY, packet =>
            {
                Console.WriteLine("received S2C_TEMP_CHATROOM_CHAT_SEND_NTY");
                _receivedMessagesTextBox.Invoke((MethodInvoker)delegate
                {
                    var chatSendNty = packet as S2C_TEMP_CHATROOM_CHAT_SEND_NTY;
                    string nickname = _userDictionary[chatSendNty.UserIdx];
                    _receivedMessagesTextBox.AppendText($"{nickname}({chatSendNty.UserIdx}) => {chatSendNty.Chat}" + Environment.NewLine);
                });
            });
        }

        private static async void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_client != null)
                {
                    C2S_TEMP_CHATROOM_LEAVE_REQ req = new C2S_TEMP_CHATROOM_LEAVE_REQ(_myUserIdx);
                    await _client.SendPacketAsync(req);
                }

                _userDictionary = new Dictionary<int, string>();
                if (_hostTextBox.Text == "" || _portTextBox.Text == "" || _nicknameTextBox.Text == "")
                {
                    MessageBox.Show("ip, port, nickname 을 모두 입력 해주세요", "에러", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                _client = new WebSocketClient(_hostTextBox.Text, int.Parse(_portTextBox.Text));
                InitCallback(_client);
                await _client.ConnectAsync();
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
                C2S_TEMP_CHATROOM_CHAT_SEND_REQ reqPacket = new C2S_TEMP_CHATROOM_CHAT_SEND_REQ(_myUserIdx, _sendMessageTextBox.Text);
                await _client.SendPacketAsync(reqPacket);
                _sendMessageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"전송 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
