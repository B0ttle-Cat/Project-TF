using System;
using System.Collections.Generic;
using System.Threading;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace TFContent.Playspace
{
	public class WorldMapBuilder : ComponentBehaviour, IWorldMapBuilder
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

		private CancellationTokenSource currentCreateTokenSource;
		private CancellationToken currentCreateToken;
		protected override void BaseAwake()
		{
			createParent = ThisTransform;
			IsValidity = false;
		}

		protected override async void BaseStart()
		{
			if(roomObjectPrefabs == null) return;
			worldMapUserSettingData = await ThisContainer.AwaitGetData<WorldMapUserSettingData>();
			worldMapBuildInfo = await ThisContainer.AwaitGetData<WorldMapBuildInfo>();
			if(worldMapUserSettingData is null || worldMapBuildInfo is null) return;

			await AwaitableUtility.WaitTrue(() => worldMapBuildInfo.worldMapRawData.Validity, DestroyCancelToken);
			IsValidity = true;
		}


		void SetWorldMapUserSettingData(WorldMapCreateDataInfo worldMapCreateDataInfo, RoomContentCreateData roomContentCreateData)
		{
			if(ThisContainer.TryGetData<WorldMapUserSettingData>(out var worldMapUserSettingData))
			{
				worldMapUserSettingData.worldMapCreateDataInfo = worldMapCreateDataInfo;
				worldMapUserSettingData.roomContentCreateData = roomContentCreateData;
			}
			else
			{
				ThisContainer.AddData<WorldMapUserSettingData>(new WorldMapUserSettingData() {
					worldMapCreateDataInfo = worldMapCreateDataInfo,
					roomContentCreateData = roomContentCreateData,
				});
			}
		}
		WorldMapRawData CreateWorldMapRawData()
		{
			WorldMapRawData worldMapRawData = worldMapBuildInfo.worldMapRawData;
			if(!ValidityRoomBuildInfo(in worldMapRawData, worldMapUserSettingData))
			{
				return worldMapRawData = WorldMapBuildInfo.LocalCreateWorldMap(worldMapUserSettingData);
			}
			return default;

			bool ValidityRoomBuildInfo(in WorldMapRawData worldMapRawData, WorldMapUserSettingData settingData)
			{
				if(settingData == null) return false;
				Vector2Int mapSizeXZ = worldMapUserSettingData.worldMapCreateDataInfo.mapSizeXZ;
				int totalSize = mapSizeXZ.x * mapSizeXZ.y;
				if(worldMapRawData.mapSize == mapSizeXZ) return false;
				else if(worldMapRawData.nodes.Length != totalSize) return false;
				else if(worldMapRawData.variantDatas.Length != totalSize) return false;
				else return true;
			}
		}
		void SetWorldMapRawData(WorldMapRawData worldMapRawData, string mapHaskey)
		{
			if(ThisContainer.TryGetData<WorldMapBuildInfo>(out var buildInfo))
			{
				buildInfo.worldMapRawData = worldMapRawData;
				buildInfo.mapHaskey = mapHaskey;
			}
			else
			{
				ThisContainer.AddData(new WorldMapBuildInfo() {
					worldMapRawData = worldMapRawData,
					mapHaskey = mapHaskey,
				});
			}
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

			currentCreateToken.ThrowIfCancellationRequested();
			AsyncInstantiateOperation async = GameObject.InstantiateAsync<RoomObject>(roomObjectPrefabs, 1, createParent, createParent.position, createParent.rotation, currentCreateToken);
			await async;
			RoomObject newRoomObject = async.Result[0] as RoomObject;
			if(prefabsActiveSelf) roomObjectPrefabs.gameObject.SetActive(true);

			return newRoomObject;
		}


		public List<int> FindNeighborNode(int centerNodeIndex, int neighborDepth)
		{
			if(worldMapBuildInfo == null) return new List<int>() { };
			if(centerNodeIndex < 0) return new List<int>() { };
			if(neighborDepth < 1) return new List<int>() { };

			List<int> findResultList = new List<int>();
			HashSet<int> findCheckList = new HashSet<int>();
			if(findCheckList.Add(centerNodeIndex))
			{
				List<int> findNextList = new List<int>();
				findNextList.AddRange(GetNeighborNodeArray(centerNodeIndex));
				_FindNeighborNode(findNextList, neighborDepth);
			}
			return findResultList;

			void _FindNeighborNode(List<int> neighborNodeList, int _findDepth)
			{
				if(_findDepth-->0)
				{
					List<int> findNextList = new List<int>();
					int length = neighborNodeList.Count;
					for(int i = 0 ; i < length ; i++)
					{
						int index = neighborNodeList[i];
						if(index >= 0 && findCheckList.Add(index))
						{
							findResultList.Add(index);

							findNextList.AddRange(GetNeighborNodeArray(index));
						}
					}
					if(findNextList.Count > 0)
					{
						_FindNeighborNode(findNextList, _findDepth);
					}
				}
			}
			int[] GetNeighborNodeArray(int nodeIndex)
			{
				if(nodeIndex<0) return null;
				if(worldMapBuildInfo == null) return null;
				WorldMapRawData.RoomNodeData[] roomNodeArray = worldMapBuildInfo.worldMapRawData.nodes;
				if(roomNodeArray.Length <= nodeIndex) return null;

				int[] neighborNodeArray = roomNodeArray[nodeIndex].NeighborNodeToArray();
				return neighborNodeArray;
			}
		}
		private async Awaitable StartCreateRoom(int createNodeIndex, int createNeighborDepth, HashSet<int> createdRooms, bool parallelCreateNeighbor, Action<List<int>> createNeighborRoom)
		{
			if(currentCreateTokenSource != null)
			{
				if(!currentCreateTokenSource.IsCancellationRequested)
				{
					currentCreateTokenSource.Cancel();
				}
				await AwaitableUtility.WaitIsNull(() => currentCreateTokenSource);
			}
			currentCreateTokenSource = new CancellationTokenSource();
			currentCreateToken = currentCreateTokenSource.Token;
			try
			{
				await CreateRoom(createNodeIndex, createNeighborDepth, createdRooms, parallelCreateNeighbor, createNeighborRoom);
			}
			catch(OperationCanceledException)
			{
				Debug.Log("새 작업이 취소되었습니다.");
			}
			currentCreateTokenSource = null;
		}
		private async Awaitable CreateRoom(int createNodeIndex, int createNeighborDepth, HashSet<int> createdRooms, bool parallelCreateNeighbor, Action<List<int>> createNeighborRoom)
		{
			if(createNodeIndex < 0) return;
			if(createNeighborDepth < 0) createNeighborDepth = 0;
			// 노드 인덱스로 룸 오브젝트 생성
			HashSet<int> _createdRooms = new HashSet<int>();
			_createdRooms.AddRange(createdRooms);

			if(_createdRooms.Add(createNodeIndex))
			{
				RoomObject roomObject = await CreateRoom(createNodeIndex);
			}

			if(createNeighborDepth == 0) return;
			if(parallelCreateNeighbor)
			{
				ParallelCreateNeighbor();
			}
			else
			{
				await CreateNeighbor();
			}

			async void ParallelCreateNeighbor() => await CreateNeighbor();
			async Awaitable CreateNeighbor()
			{
				List<int> findNeighborNodeList = FindNeighborNode(createNodeIndex, createNeighborDepth);
				int length = findNeighborNodeList.Count;
				for(int i = 0 ; i < length ; i++)
				{
					int neighborIndex = findNeighborNodeList[i];
					if(_createdRooms.Add(neighborIndex))
					{
						RoomObject roomObject = await CreateRoom(neighborIndex);
					}
				}
				createNeighborRoom?.Invoke(findNeighborNodeList);
			}
		}
		private void CancelCreate()
		{
			if(!currentCreateTokenSource.IsCancellationRequested) currentCreateTokenSource.Cancel();
			currentCreateTokenSource = null;
		}

		void IWorldMapBuilder.SetWorldMapUserSettingData(WorldMapCreateDataInfo worldMapCreateDataInfo, RoomContentCreateData roomContentCreateData) => SetWorldMapUserSettingData(worldMapCreateDataInfo, roomContentCreateData);
		WorldMapRawData IWorldMapBuilder.CreateWorldMapRawData() => CreateWorldMapRawData();
		void IWorldMapBuilder.SetWorldMapRawData(WorldMapRawData worldMapRawData, string mapHash) => SetWorldMapRawData(worldMapRawData, mapHash);
		async Awaitable IWorldMapBuilder.CreateRoom(int createNodeIndex, HashSet<int> createdRooms) => await StartCreateRoom(createNodeIndex, 0, createdRooms, false, null);
		async Awaitable IWorldMapBuilder.CreateRoom(int createNodeIndex, int createNeighborDepth, HashSet<int> createdRooms, bool parallelCreateNeighbor, Action<List<int>> createNeighborRoom) => await StartCreateRoom(createNodeIndex, createNeighborDepth, createdRooms, parallelCreateNeighbor, createNeighborRoom);
		void IWorldMapBuilder.CancelCreate() => CancelCreate();
	}
}
