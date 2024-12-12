using System;
using System.Collections.Generic;

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

			LocalCreateWorldMap();
			await UploadWorld();

			IsValidity = true;

			// 테스트로 전체 룸 생성.
			// 원래는 유저 정보의 사용자 위치를 통해서 생성해야 함.
			if(IsValidity)
			{
				Vector2Int mapSizeXZ = worldMapUserSettingData.mapSizeXZ;
				int size = mapSizeXZ.x * mapSizeXZ.y;
				for(int i = 0 ; i < size ; i++)
				{
					await CreateRoom(i);
				}
			}
		}

		private void LocalCreateWorldMap()
		{
			WorldMapRawData worldMapRawData = worldMapBuildInfo.worldMapRawData;
			if(!ValidityRoomBuildInfo(in worldMapRawData, worldMapUserSettingData))
			{
				worldMapBuildInfo.worldMapRawData = worldMapRawData = WorldMapBuildInfo.LocalCreateWorldMap(worldMapUserSettingData);
			}

			bool ValidityRoomBuildInfo(in WorldMapRawData worldMapRawData, WorldMapUserSettingData settingData)
			{
				if(settingData == null) return false;
				int totalSize = settingData.mapSizeXZ.x * settingData.mapSizeXZ.y;
				if(worldMapRawData.mapSize == settingData.mapSizeXZ) return false;
				else if(worldMapRawData.nodes.Length != totalSize) return false;
				else if(worldMapRawData.variantDatas.Length != totalSize) return false;
				else return true;
			}
		}
		private async Awaitable<bool> UploadWorld()
		{
			await Awaitable.NextFrameAsync();
			return true;
		}
		private async Awaitable<bool> DownloadWorld()
		{
			await Awaitable.NextFrameAsync();
			return true;
		}
		private async Awaitable<RoomObject> CreateRoom(Vector2Int tableIndex)
		{
			if(!IsValidity) return null;
			var mapSize = worldMapBuildInfo.worldMapRawData.mapSize;
			int nodeIndex = tableIndex.y * mapSize.x + tableIndex.x;
			return await CreateRoom(nodeIndex);
		}

		private async Awaitable<RoomObject> CreateRoom(int nodeIndex)
		{
			if(!IsValidity) return null;
			if(nodeIndex < 0) return null;

			var worldMapRawData = worldMapBuildInfo.worldMapRawData;
			// 기본적인 유효성 검사
			if(worldMapRawData.nodes.Length <= nodeIndex) return null;
			if(worldMapRawData.variantDatas.Length <= nodeIndex) return null;
			var rawRoomNodeData = worldMapRawData.nodes[nodeIndex];
			var rawRoomVariationData = worldMapRawData.variantDatas[nodeIndex];
			if(nodeIndex != rawRoomNodeData.nodeIndex) return null;

			// 빈 룸 오브젝트 생성
			RoomObject roomObject = await CreateRoomObject();
			if(roomObject == null) return null;

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
				new() { linkIndex = rawRoomNodeData.xNodeIndex, linkDir = LinkInfo.NodeDir.X_Dir },
				new() { linkIndex = rawRoomNodeData.yNodeIndex, linkDir = LinkInfo.NodeDir.Y_Dir },
				new() { linkIndex = rawRoomNodeData.iXNodeIndex, linkDir = LinkInfo.NodeDir.iX_Dir },
				new() { linkIndex = rawRoomNodeData.iYNodeIndex, linkDir = LinkInfo.NodeDir.iY_Dir },
			};

			roomVariationData.roomThemeName = rawRoomVariationData.themeName;
			roomVariationData.roomContentType = rawRoomVariationData.RoomContentType;
			roomVariationData.roomRandomSeed = rawRoomVariationData.randomSeed;

			// 준비가 완료되면 활성화 한다.
			if(!roomObject.GameObject.activeSelf)
			{
				roomObject.GameObject.SetActive(true);
			}
			return roomObject;
		}

		private async Awaitable<RoomObject> CreateRoomObject()
		{
			if(!IsValidity) return null;

			// 기본적으로 비활성화 상태로 생성되게 한다.
			bool prefabsActiveSelf = roomObjectPrefabs.GameObject.activeSelf;
			if(prefabsActiveSelf) roomObjectPrefabs.gameObject.SetActive(false);
			AsyncInstantiateOperation async = GameObject.InstantiateAsync<RoomObject>(roomObjectPrefabs, createParent);
			await async;
			RoomObject newRoomObject = async.Result[0] as RoomObject;
			if(prefabsActiveSelf) roomObjectPrefabs.gameObject.SetActive(true);

			return newRoomObject;
		}

		public List<int> FindNeighborNode(int nodeIndex, int findDepth)
		{
			if(worldMapBuildInfo == null) return null;
			if(nodeIndex<0) return null;
			if(findDepth<0) findDepth = 0;

			List<int> findResultList = new List<int>();
			HashSet<int> findCheckList = new HashSet<int>();
			if(findCheckList.Add(nodeIndex))
			{
				List<int> findNextList = new List<int>();
				findNextList.AddRange(GetNeighborNodeArray(nodeIndex));
				_FindNeighborNode(findNextList, findDepth);
			}
			return findResultList;

			void _FindNeighborNode(List<int> neighborNodeList, int _findDepth)
			{
				_findDepth--;
				if(_findDepth>0)
				{
					List<int> findNextList = new List<int>();
					int length = neighborNodeList.Count;
					for(int i = 0 ; i < length ; i++)
					{
						int index = neighborNodeList[i];
						if(findCheckList.Add(index))
						{
							findResultList.Add(index);

							findNextList.AddRange(GetNeighborNodeArray(nodeIndex));
						}
					}
					if(findNextList.Count > 0)
					{
						_FindNeighborNode(findNextList, _findDepth);
					}
				}
			}
		}
		public async void CreateRoom(int nodeIndex, int createDepth, HashSet<int> createdRooms, Action createRoom, Action<List<int>> createNeighborRoom)
		{
			if(nodeIndex < 0) return;
			if(createDepth < 0) createDepth = 0;
			// 노드 인덱스로 룸 오브젝트 생성
			if(createdRooms.Add(nodeIndex))
			{
				RoomObject roomObject = await CreateRoom(nodeIndex);
			}
			createRoom?.Invoke();

			List<int> findNeighborNodeList = FindNeighborNode(nodeIndex, createDepth);
			int length = findNeighborNodeList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				int neighborIndex = findNeighborNodeList[i];
				if(createdRooms.Add(neighborIndex))
				{
					RoomObject roomObject = await CreateRoom(neighborIndex);
				}
			}
			createNeighborRoom?.Invoke(findNeighborNodeList);
		}
		internal int[] GetNeighborNodeArray(int nodeIndex)
		{
			if(nodeIndex<0) return null;
			if(worldMapBuildInfo == null) return null;
			WorldMapRawData.RoomNodeData[] roomNodeArray = worldMapBuildInfo.worldMapRawData.nodes;
			if(roomNodeArray.Length <= nodeIndex) return null;

			int[] neighborNodeArray = roomNodeArray[nodeIndex].NeighborNodeToArray();
			return neighborNodeArray;
		}
	}
}
