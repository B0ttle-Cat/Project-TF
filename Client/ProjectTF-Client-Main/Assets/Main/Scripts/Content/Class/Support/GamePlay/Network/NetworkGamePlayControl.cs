using System.Linq;

using BC.Base;
using BC.ODCC;

using TFSystem;
using TFSystem.Network;

using UnityEngine;
namespace TFContent
{
	public class NetworkGamePlayControl : ComponentBehaviour, INetworkAPI.GamePlayAPI
	{
		QuerySystem queryGamePlayUser;
		OdccQueryCollector collectorGamePlayUser;

		PacketNotifyItem userEnterNty;
		PacketNotifyItem userLeaveNty;


		public bool IsEnterGameRoom { get; private set; }
		public IApplicationController AppController { get; private set; }
		public INetworkController NetworkController => AppController.NetworkController;
		public IWorldMapSystem WorldMapSystem { get; private set; }

		protected override void BaseAwake()
		{
			IsEnterGameRoom = false;

			ThisContainer.TryGetObject<GamePlaySystem>(out var systemState);
			AppController = systemState.AppController;

			userLeaveNty = PacketAsyncItem.CreateNotifyItem<S2C_GAMEROOM_ENTER_NTY>(UserLeaveNty, true);

			queryGamePlayUser = QuerySystemBuilder.CreateQuery()
				.WithAll<NetworkUser, UserBaseData, UserGamePlayData, UserGamePlayer>().Build();
			collectorGamePlayUser = OdccQueryCollector.CreateQueryCollector(queryGamePlayUser)
				.GetCollector();
		}
		protected override void BaseDestroy()
		{
			AppController = null;
			WorldMapSystem = null;
			if(queryGamePlayUser != null)
			{
				OdccQueryCollector.DeleteQueryCollector(queryGamePlayUser);
				queryGamePlayUser = null;
				collectorGamePlayUser = null;
			}
		}
		private async Awaitable<bool> OnEnterGameAsync(INetworkUser networkUser)
		{
			if(IsEnterGameRoom) return true;
			if(networkUser == null) return false;
			if(!networkUser.ThisContainer.TryGetData<UserBaseData>(out var userBaseData)) return false;
			if(userBaseData.NetworkState != UserBaseData.NetworkStateType.Connect) return false;
			if(NetworkController.IsDisconnect) return false;


			WorldMapSystem = await AppController.SystemController.AwaitGetSystemState<IWorldMapSystem>(ThisObject.DestroyCancelToken);
			if(WorldMapSystem == null) return false;

			// 생성에 필요한 정보 세팅
			SetWorldMapUserSetting();
			void SetWorldMapUserSetting()
			{
				// SetWorldMapUserSettingData
				WorldMapCreateDataInfo worldMapCreateDataInfo = AppController.DataCarrier.PopData<WorldMapCreateDataInfo>(nameof(WorldMapCreateDataInfo),
				new WorldMapCreateDataInfo(){
					mapSeed = Random.Range(int.MinValue, int.MaxValue),
					mapSizeXZ = new Vector2Int(8,8),
					multipathRate = 0.4f
				});
				RoomContentCreateData roomContentCreateData = AppController.DataCarrier.PopData<RoomContentCreateData>(nameof(RoomContentCreateData),
				new RoomContentCreateData(){
					roomThemeName = "DefaultTheme",
					contentPoint = new System.Collections.Generic.List<RoomContentCreateData.ContentPoint>(),
				});
				WorldMapSystem.WorldMapBuilder.SetWorldMapUserSettingData(worldMapCreateDataInfo, roomContentCreateData);
			}

			// 로털에서 태스트용으로 맵 생성후 저장소에 업로드
			string mapHashKey = await LocalCreateMapTest();
			async Awaitable<string> LocalCreateMapTest()
			{
				WorldMapRawData worldMapRawData_beforeUpload = WorldMapSystem.WorldMapBuilder.CreateWorldMapRawData();
				if(!worldMapRawData_beforeUpload.Validity) return "";

				return await MapRepositoryAPI.Editor_UploadWorldMapRawData(worldMapRawData_beforeUpload);
			}
			if(string.IsNullOrWhiteSpace(mapHashKey)) return false;

			// 게임룸 입장 패킷 전송 및 맵 데이터 수신
			WorldMapRawData worldMapRawData = await SendGameRoomEnterPacket(mapHashKey);
			async Awaitable<WorldMapRawData> SendGameRoomEnterPacket(string mapHashKey)
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_GAMEROOM_ENTER_ACK>(new C2S_GAMEROOM_ENTER_REQ {
					userIdx = userBaseData.UserIdx,
					nickname = userBaseData.Nickname,
					mapHash = mapHashKey,
				});
				if(receive == null || receive.Failure) return default;

				WorldMapRawData worldMapRawData = new WorldMapRawData(){
					mapSize = new Vector2Int(receive.mapSize.x,receive.mapSize.y),
					nodes = receive.nodes.Select(node =>
					new WorldMapRawData.RoomNodeData
					{
						nodeIndex = node.nodeIndex,
						tableIndex = new Vector2Int(node.tableIndex.x,node.tableIndex.y),
						iXNodeIndex = node.iXNodeIndex,
						iYNodeIndex = node.iYNodeIndex,
						xNodeIndex = node.xNodeIndex,
						yNodeIndex = node.yNodeIndex,
					}).ToArray(),
					variantDatas = receive.variantDatas.Select(variantData =>
					new WorldMapRawData.RoomVariationData
					{
						themeName = variantData.themeName,
						contentType = variantData.contentType,
						randomSeed = variantData.randomSeed,
					}).ToArray(),
				};
				return worldMapRawData;
			}
			if(!worldMapRawData.Validity) return false;

			// 맵 데이터 세팅
			WorldMapSystem.WorldMapBuilder.SetWorldMapRawData(worldMapRawData, mapHashKey);
			await AwaitableUtility.WaitTrue(() => WorldMapSystem.WorldMapBuilder.IsValidity, ThisObject.DestroyCancelToken);

			// 플레이어가 맵에서 어는 지점에 있는지 확인.
			int playerNodeIndex = await SendReqPlayerGamePlayDataPacket();
			int createNeighborDepth = 2;
			async Awaitable<int> SendReqPlayerGamePlayDataPacket()
			{
				await Awaitable.NextFrameAsync();
				int randomNodeIndex = Random.Range(0, worldMapRawData.mapSize.x * worldMapRawData.mapSize.y);
				return randomNodeIndex;
			}
			if(playerNodeIndex<0) return false;

			// 확인된 플레이어 위치를 토대로 맵을 생성
			IRoomObject roomObject = await CreateWorldMapRoom(playerNodeIndex, createNeighborDepth);
			async Awaitable<IRoomObject> CreateWorldMapRoom(int centerNodeIndex, int createNeighborDepth)
			{
				WorldMapSystem.RoomObjectGroup.CreateNeighborDepth = createNeighborDepth;
				return await WorldMapSystem.RoomObjectGroup.CreateRoom(centerNodeIndex, null);
			}
			if(roomObject == null) return false;


			networkUser.ThisContainer.AddComponent<UserGamePlayer>();
			networkUser.ThisContainer.AddData<UserGamePlayData>();

			IsEnterGameRoom = true;
			return true;
		}
		private async Awaitable OnLeaveGameAsync(INetworkUser networkUser)
		{
			if(!IsEnterGameRoom) return;
			IsEnterGameRoom = false;


			// TODO:: 이제 나가는작업 하면 됨.
			await SendGameRoomnLeavePacket();
			async Awaitable SendGameRoomnLeavePacket()
			{
				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_GAMEROOM_ENTER_ACK>(new C2S_GAMEROOM_ENTER_REQ {
					userIdx = -1,
					nickname = "",
					mapHash = "",
				});
			}


			networkUser.ThisContainer.RemoveComponent<UserGamePlayer>();
			networkUser.ThisContainer.RemoveData<UserGamePlayData>();
		}
		private void UserEnterNty(S2C_GAMEROOM_ENTER_ACK aCK)
		{
		}
		private void UserLeaveNty(S2C_GAMEROOM_ENTER_NTY nTY)
		{
		}



		async Awaitable<bool> INetworkAPI.GamePlayAPI.OnEnterGameAsync(INetworkUser networkUser) => await OnEnterGameAsync(networkUser);
		async Awaitable INetworkAPI.GamePlayAPI.OnLeaveGameAsync(INetworkUser networkUser) => await OnLeaveGameAsync(networkUser);
	}
}