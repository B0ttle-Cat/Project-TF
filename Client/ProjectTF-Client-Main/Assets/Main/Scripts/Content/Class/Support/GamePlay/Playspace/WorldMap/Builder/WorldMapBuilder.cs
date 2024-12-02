using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TFContent.Playspace
{
	public class WorldMapBuilder : ComponentBehaviour
	{
		[ShowInInspector, ReadOnly]
		private WorldMapUserSettingData worldMapUserSettingData;

		[ShowInInspector, ReadOnly]
		private WorldMapBuildInfo worldMapBuildInfo;
		[ShowInInspector, ReadOnly]
		public bool IsValidity { get; private set; }

		[ShowInInspector, ReadOnly]
		private Transform createParent;

		[SerializeField]
		private RoomObject roomObjectPrefabs;


		[SerializeField]
		[Range(15,20)]
		private float roomSpaceDistance = 15;

		protected override void BaseAwake()
		{
			createParent = ThisTransform;
			IsValidity = false;
		}

		protected override async void BaseStart()
		{
			IsValidity = false;
			if(roomObjectPrefabs == null) return;
			worldMapUserSettingData = await ThisContainer.AwaitGetData<WorldMapUserSettingData>();
			worldMapBuildInfo = await ThisContainer.AwaitGetData<WorldMapBuildInfo>();
			if(worldMapUserSettingData is null || worldMapBuildInfo is null) return;
			IsValidity = true;

			CreateWorld();

			// 전체 룸 생성
			if(IsValidity)
			{
				Vector2Int mapSizeXZ = worldMapUserSettingData.mapSizeXZ;
				int size = mapSizeXZ.x * mapSizeXZ.y;
				for(int i = 0 ; i < size ; i++)
				{
					CreateRoom(i);
				}
			}
		}
		private void CreateWorld()
		{
			WorldMapRawData worldMapRawData = worldMapBuildInfo.worldMapRawData;
			if(!ValidityRoomBuildInfo(in worldMapRawData, worldMapUserSettingData))
			{
				worldMapBuildInfo.worldMapRawData = worldMapRawData = WorldMapRawData.CreateSample(worldMapUserSettingData);
			}

			bool ValidityRoomBuildInfo(in WorldMapRawData worldMapRawData, WorldMapUserSettingData settingData)
			{
				if(settingData == null) return false;
				int totalSize = settingData.mapSizeXZ.x * settingData.mapSizeXZ.y;
				if(worldMapRawData.seed != settingData.mapSeed) return false;
				else if(worldMapRawData.mapSize == settingData.mapSizeXZ) return false;
				else if(Mathf.Abs(worldMapRawData.multipathRate - settingData.multipathRate) > float.Epsilon) return false;
				else if(worldMapRawData.roomNodeArray.Length != totalSize) return false;
				else if(worldMapRawData.roomVariationDataArray.Length != totalSize) return false;
				else return true;
			}
		}
		public void CreateRoom(Vector2Int tableIndex)
		{
			if(!IsValidity) return;
			var mapSize = worldMapBuildInfo.worldMapRawData.mapSize;
			int nodeIndex = tableIndex.y * mapSize.x + tableIndex.x;
			CreateRoom(nodeIndex);
		}

		public void CreateRoom(int nodeIndex)
		{
			if(!IsValidity) return;
			var worldMapRawData = worldMapBuildInfo.worldMapRawData;

			// 기본적인 유효성 검사
			if(worldMapRawData.roomNodeArray.Length <= nodeIndex) return;
			if(worldMapRawData.roomVariationDataArray.Length <= nodeIndex) return;
			var rawRoomNodeData = worldMapRawData.roomNodeArray[nodeIndex];
			var rawRoomVariationData = worldMapRawData.roomVariationDataArray[nodeIndex];
			if(nodeIndex != rawRoomNodeData.nodeIndex) return;

			// 빈 룸 오브젝트 생성
			RoomObject roomObject = CreateRoomObject();
			if(roomObject == null) return;

			// 룸 오브젝트 위치 설정
			Vector2Int tableIndex = rawRoomNodeData.tableIndex;
			var roomTransform = roomObject.ThisTransform;
			roomTransform.position = new Vector3(tableIndex.x * roomSpaceDistance, 0, tableIndex.y * roomSpaceDistance);
			roomTransform.rotation = Quaternion.identity;
			roomTransform.localScale = Vector3.one;

			// 룸 데이터 확인 또는 생성
			if(!roomObject.ThisContainer.TryGetData<RoomNodeData>(out RoomNodeData roomNodeDataList))
			{
				roomNodeDataList = roomObject.ThisContainer.AddData<RoomNodeData>();
			}
			if(!roomObject.ThisContainer.TryGetData<RoomVariationData>(out RoomVariationData roomVariationData))
			{
				roomVariationData = roomObject.ThisContainer.AddData<RoomVariationData>();
			}

			// 룸 오브젝트에 초기화 값 세팅
			roomNodeDataList.nodeIndex = rawRoomNodeData.nodeIndex;
			roomNodeDataList.tableIndex = rawRoomNodeData.tableIndex;
			roomNodeDataList.linkList = new LinkInfo[4] {
				new() { linkIndex = rawRoomNodeData.XNodeIndex, linkDir = LinkInfo.NodeDir.X_Dir },
				new() { linkIndex = rawRoomNodeData.YNodeIndex, linkDir = LinkInfo.NodeDir.Y_Dir },
				new() { linkIndex = rawRoomNodeData.iXNodeIndex, linkDir = LinkInfo.NodeDir.iX_Dir },
				new() { linkIndex = rawRoomNodeData.iYNodeIndex, linkDir = LinkInfo.NodeDir.iY_Dir },
			};

			roomVariationData.roomThemeName = rawRoomVariationData.roomThemeName;
			roomVariationData.roomContentType = rawRoomVariationData.RoomContentType;
			roomVariationData.roomRandomSeed = rawRoomVariationData.roomRandomSeed;

			// 준비가 완료되면 활성화 한다.
			if(!roomObject.GameObject.activeSelf)
			{
				roomObject.GameObject.SetActive(true);
			}
		}

		private RoomObject CreateRoomObject()
		{
			if(!IsValidity) return null;

			// 기본적으로 비활성화 상태로 생성되게 한다.
			bool prefabsActiveSelf = roomObjectPrefabs.GameObject.activeSelf;
			if(prefabsActiveSelf) roomObjectPrefabs.gameObject.SetActive(false);
			RoomObject newRoomObject = GameObject.Instantiate(roomObjectPrefabs, createParent);
			if(prefabsActiveSelf) roomObjectPrefabs.gameObject.SetActive(true);

			return newRoomObject;
		}
	}
}
