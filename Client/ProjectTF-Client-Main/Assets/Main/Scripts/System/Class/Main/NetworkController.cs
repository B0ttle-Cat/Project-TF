using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

using Debug = UnityEngine.Debug;
namespace TFSystem.Network
{
	/*
	WebSocketCloseStatus 
	상태	코드	설명	예시
	NormalClosure			1000	연결이 정상적으로 종료됨	파일 전송 완료 후 연결 종료
	EndpointUnavailable		1001	상대방이 더 이상 사용 불가	서버 다운, 클라이언트 종료
	ProtocolError			1002	프로토콜 위반	잘못된 WebSocket 메시지 형식
	InvalidMessageType		1003	지원되지 않는 메시지 타입	비정상 메시지 프레임 전송
	Empty					1005	종료 상태 코드가 없음	명시되지 않은 종료 이유
	InvalidPayloadData		1007	잘못된 데이터 형식	UTF-8 인코딩 오류
	PolicyViolation			1008	정책 위반	인증 실패, 금지된 작업 수행
	MessageTooBig			1009	메시지가 너무 큼	1MB 초과 메시지 전송
	MandatoryExtension		1010	필수 확장이 협상되지 않음	클라이언트가 확장을 요구했으나 서버가 거부
	InternalServerError		1011	서버에서 예기치 않은 내부 오류 발생	메모리 부족, 예외 발생
	TLSHandshakeFailure		1015	TLS/SSL 핸드셰이크 실패 (비공식)	인증서 오류, 보안 연결 실패
	*/
	/*
	WebSocketState
	상태	설명	가능한 동작
	None			WebSocket이 초기화된 상태. 연결 시도 전. ConnectAsync 호출 가능
	Connecting		연결 시도 중.	연결 성공 시 Open으로 전환
	Open			연결 성공. 데이터 송수신 가능.	CloseAsync 호출 또는 데이터 송수신 가능
	CloseSent		클라이언트가 종료 요청.	서버가 종료 확인 프레임을 보내면 Closed로 전환
	CloseReceived	서버가 종료 요청.	클라이언트가 종료 확인 프레임을 보내면 Closed로 전환
	Closed			연결이 정상적으로 종료됨.	WebSocket 객체를 더 이상 사용할 수 없음
	Aborted			연결이 비정상적으로 종료됨.	WebSocket 객체를 더 이상 사용할 수 없음
	 */

	public class NetworkController : ComponentBehaviour, INetworkController, IOdccUpdate, INetworkSendEvent
	{
		[ShowInInspector, DisplayAsString]
		public string NetworkIp => "20.41.121.220";
		[ShowInInspector, DisplayAsString]
		public string NetworkPort => "38201";
		[ShowInInspector, DisplayAsString]
		public string NetworkURL => $"ws://{NetworkIp}:{NetworkPort}";

		[ShowInInspector]
		private bool ShowLog { get; set; } = false;

		private ClientWebSocket webSocket;
		private Queue<Action> actionReceiveHandler;

		public enum WebSocketBufferSize
		{
			Small = 1024 / 2,       // 작은 메시지 (텍스트 기반 소규모 데이터)
			Medium = 1024,          // 일반적인 메시지 크기 (텍스트 및 경량 데이터)
			Large = 1024 * 2,       // 큰 메시지 (JSON, XML, 이미지 데이터)
			ExtraLarge = 1024 * 4,  // 아주 큰 메시지 (대용량 바이너리)
		}
		private void LogException(Exception ex)
		{
			if(ShowLog) Debug.LogError($"NetworkLog:Exception: {ex.Message}");
			Debug.LogException(ex);
		}
		private void LogError(string log)
		{
			if(ShowLog) Debug.LogError($"NetworkLog:Error: {log}");
		}
		private void Log(string log)
		{
			if(ShowLog) Debug.Log($"NetworkLog: {log}");
		}

		protected override void BaseAwake()
		{
			int uniqueId = (int)DateTime.UtcNow.Ticks;
		}
		protected override void BaseDestroy()
		{
			if(webSocket == null) return;

			var tempWebSocket = webSocket;
			webSocket = null;

			Task.Run(async () => {
				switch(tempWebSocket.State)
				{
					case WebSocketState.Open:
					case WebSocketState.Connecting:
					case WebSocketState.CloseReceived:
					{
						await tempWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application Close", CancellationToken.None);
						break;
					}
					default: break;
				}
			});
		}



		private async Task ConnectAsync()
		{
			webSocket ??= new ClientWebSocket();
			if(webSocket.State == WebSocketState.Closed)
			{
				webSocket = new ClientWebSocket();
			}

			var tempWebSocket = webSocket;
			bool isFirst = false;

			if(tempWebSocket.State == WebSocketState.None)
			{
				try
				{
					Log("ConnectAsync-Start");
					await tempWebSocket.ConnectAsync(new Uri(NetworkURL), CancellationToken.None);
					Log("ConnectAsync-Ended");
					isFirst = true;
					ReceiveAsync();
				}
				catch(Exception ex)
				{
					LogException(ex);
					return;
				}
			}
			else if(tempWebSocket.State == WebSocketState.Connecting)
			{
				while(tempWebSocket.State == WebSocketState.Connecting)
				{
					await Awaitable.WaitForSecondsAsync(1f);
				}
			}
		}
		private async Task CloseAsync()
		{
			if(webSocket == null) return;
			var tempWebSocket = webSocket;
			webSocket = null;

			await Task.Run(async () => {
				switch(tempWebSocket.State)
				{
					case WebSocketState.Open:
					case WebSocketState.Connecting:
					case WebSocketState.CloseReceived:
					{
						Log("Application Close");
						await tempWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application Close", CancellationToken.None);
						break;
					}
					default: break;
				}
			});
		}
		private async Task SendAsync<T>(T packetData) where T : class, IPacketSend
		{
			if(webSocket == null) return;
			var tempWebSocket = webSocket;

			try
			{
				string json = IPacketSend.ToJson(packetData);
				Log($"Send: {json}");
				byte[] buffer = Encoding.UTF8.GetBytes(json);
				Log($"SendAsync-Start: {json}");
				await tempWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
				Log($"SendAsync-Ended");
			}
			catch(Exception ex)
			{
				LogException(ex);
			}
		}
		private void ReceiveAsync()
		{
			ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[(int)WebSocketBufferSize.ExtraLarge]);
			Task.Run(async () => {
				while(webSocket != null && webSocket.State == WebSocketState.Open)
				{
					var tempWebSocket = webSocket;
					try
					{
						WebSocketReceiveResult result = null;
						using(var ms = new MemoryStream())
						{
							do
							{
								result = await tempWebSocket.ReceiveAsync(buffer, CancellationToken.None);
								ms.Write(buffer.Array, buffer.Offset, result.Count);
							}
							while(!result.EndOfMessage);

							ms.Seek(0, SeekOrigin.Begin);

							if(result.MessageType == WebSocketMessageType.Close)
							{
								Log("Receive Close Message");
								await tempWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Receive Close Message", CancellationToken.None);
							}
							else if(result.MessageType == WebSocketMessageType.Text)
							{
								Log("Receive Text Message");
								byte[] data = ms.ToArray();
								string json = Encoding.UTF8.GetString(data);
								Log($"Receive Json: {json}");
								SwitchReceiveProtocol(json);
							}
						}
					}
					catch(Exception ex)
					{
						LogException(ex);
					}
				}
			});
		}
		private void SwitchReceiveProtocol(in string json)
		{
			if(!IPacketReceive.TryS2CCode(in json, out var _S2C)) return;

			switch(_S2C)
			{
				case Protocol.S2C.S2C_TEMP_CHATROOM_ENTER_ACK: CallReceiveHandler<S2C_TEMP_CHATROOM_ENTER_ACK>(in json); break;
				case Protocol.S2C.S2C_TEMP_CHATROOM_ENTER_NTY: CallReceiveHandler<S2C_TEMP_CHATROOM_ENTER_NTY>(in json); break;
				case Protocol.S2C.S2C_TEMP_CHATROOM_LEAVE_ACK: CallReceiveHandler<S2C_TEMP_CHATROOM_LEAVE_ACK>(in json); break;
				case Protocol.S2C.S2C_TEMP_CHATROOM_LEAVE_NTY: CallReceiveHandler<S2C_TEMP_CHATROOM_LEAVE_NTY>(in json); break;
				case Protocol.S2C.S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK: CallReceiveHandler<S2C_TEMP_CHATROOM_SNAPSHOT_GET_ACK>(in json); break;
				case Protocol.S2C.S2C_TEMP_CHATROOM_CHAT_SEND_ACK: CallReceiveHandler<S2C_TEMP_CHATROOM_CHAT_SEND_ACK>(in json); break;
				case Protocol.S2C.S2C_TEMP_CHATROOM_CHAT_SEND_NTY: CallReceiveHandler<S2C_TEMP_CHATROOM_CHAT_SEND_NTY>(in json); break;
				default: break;
			};
		}
		private void CallReceiveHandler<T>(in string json) where T : class, IPacketReceive
		{
			IPacketReceive packetReceive = IPacketReceive.FromJson<T>(json);
			if(packetReceive == null || packetReceive is not T packetData) return;

			Log($"CallReceiveHandler: {typeof(T).Name}");
			actionReceiveHandler ??= new Queue<Action>();
			actionReceiveHandler.Enqueue(() => EventManager.Call<INetworkReceiveHandler<T>>(call => call.OnReceive(packetData)));
		}

		async Awaitable<bool> INetworkController.OnConnectAsync()
		{
			await ConnectAsync();
			await Awaitable.MainThreadAsync();
			return webSocket != null && webSocket.State == WebSocketState.Open;
		}
		async Awaitable INetworkController.OnDisconnectAsync()
		{
			await CloseAsync();
			await Awaitable.MainThreadAsync();
		}
		async Awaitable INetworkSendEvent.OnSendAsync<T>(T packetData)
		{
			await SendAsync<T>(packetData);
			await Awaitable.MainThreadAsync();
		}

		void IOdccUpdate.BaseUpdate()
		{
			if(actionReceiveHandler == null || actionReceiveHandler.Count == 0) return;

			while(actionReceiveHandler.Count > 0)
			{
				try
				{
					actionReceiveHandler.Dequeue()?.Invoke();
				}
				catch(Exception ex)
				{
					LogException(ex);
				}
			}
		}
	}
}