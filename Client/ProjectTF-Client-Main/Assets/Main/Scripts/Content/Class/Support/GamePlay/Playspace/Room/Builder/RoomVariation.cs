using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace TFContent.Playspace
{
	public class RoomVariation : ComponentBehaviour//, IOdccUpdate
	{
		[SerializeField]
		private GameObject defaultFloor;

		private const float ModelRealSize = 5f/4f;
		private const int BlockSize = 4;
		private const int HalfBlockSize = 2;
		private const int QuarterBlockSize = 1;

		private static Vector2Int floorCount = new Vector2Int(3, 3);

		private static readonly int[] WallModelSize = new int[3] { QuarterBlockSize, HalfBlockSize, BlockSize };
		private const int DoorModelSize = 4;
		private static readonly int[] CornerWallSize = new int[2] {2, 4};

		private const int DoorWallID = -1;
		private const int CornerWallID = -2;

		private const int EmptyWallID = -99;

		public FloorVariationRawData floorVariationRawData;
		public WallVariationRawData wallVariationRawDataX;
		public WallVariationRawData wallVariationRawDataY;
		public WallVariationRawData wallVariationRawDataIX;
		public WallVariationRawData wallVariationRawDataIY;
		///	        -(_Y)- 
		///	<-(iX)|        | (_X)->
		///	        -(iY)-
		///	        
		//protected override async void BaseEnable()
		//{
		//	StartRoomVariation();
		//	await CreateRoomResources();
		//}

		//protected override void BaseDisable()
		//{
		//	ClearRoomResources();
		//	ClearProResources();
		//}

		[ButtonGroup, Button(Name = "Start Variation")]
		public void StartRoomVariation()
		{
			if(!ThisContainer.TryGetData<RoomVariationData>(out var variationData)) return;

			var oldState = Random.state;
			Random.InitState(variationData.roomRandomSeed);
			FloorVariation();
			WallVariation();
			PropVariation();
			Random.state = oldState;

			void FloorVariation()
			{
				floorVariationRawData = new FloorVariationRawData(new Vector2Int(4, 4));
				floorVariationRawData.SetFloorCount(floorCount);
			}
			void WallVariation()
			{
				Vector2Int roomFloorSize = floorVariationRawData.RoomFloorSize;

				int roomLengthX = roomFloorSize.x;
				int roomLengthY = roomFloorSize.y;

				wallVariationRawDataX = new WallVariationRawData(roomLengthY);
				wallVariationRawDataY = new WallVariationRawData(roomLengthX);
				wallVariationRawDataIX = new WallVariationRawData(roomLengthY);
				wallVariationRawDataIY = new WallVariationRawData(roomLengthX);

				if(ThisContainer.TryGetData<RoomNodeData>(out var nodeData))
				{
					if(nodeData.linkList[0].linkIndex >= 0) wallVariationRawDataX.SetDoor(DoorModelSize, DoorWallID);
					if(nodeData.linkList[1].linkIndex >= 0) wallVariationRawDataY.SetDoor(DoorModelSize, DoorWallID);
					if(nodeData.linkList[2].linkIndex >= 0) wallVariationRawDataIX.SetDoor(DoorModelSize, DoorWallID);
					if(nodeData.linkList[3].linkIndex >= 0) wallVariationRawDataIY.SetDoor(DoorModelSize, DoorWallID);
				}

				wallVariationRawDataX.SplitWall(WallModelSize);
				wallVariationRawDataY.SplitWall(WallModelSize);
				wallVariationRawDataIX.SplitWall(WallModelSize);
				wallVariationRawDataIY.SplitWall(WallModelSize);

				MergeCornerWall(ref wallVariationRawDataX, ref wallVariationRawDataY);
				MergeCornerWall(ref wallVariationRawDataY, ref wallVariationRawDataIX);
				MergeCornerWall(ref wallVariationRawDataIX, ref wallVariationRawDataIY);
				MergeCornerWall(ref wallVariationRawDataIY, ref wallVariationRawDataX);

				MergeWallQuarterAndHalf(ref wallVariationRawDataX);
				MergeWallQuarterAndHalf(ref wallVariationRawDataY);
				MergeWallQuarterAndHalf(ref wallVariationRawDataIX);
				MergeWallQuarterAndHalf(ref wallVariationRawDataIY);

				void MergeCornerWall(ref WallVariationRawData ab, ref WallVariationRawData bc)
				{
					if(Random.Range(0, 2)==0) return;

					var abWall = ab.GetWallList();
					var bcWall = bc.GetWallList();

					if(abWall.Count == 0 || bcWall.Count == 0) return;

					(int wallID, int startIndex, int wallCount) _b = abWall[^1];
					(int wallID, int startIndex, int wallCount) b_ = bcWall[0];

					if(_b.wallCount == b_.wallCount && (CornerWallSize.Any(c => c == _b.wallCount)))
					{
						int _bCount = _b.startIndex + _b.wallCount;
						for(int i = _b.startIndex ; i < _bCount ; i++)
						{
							ab.wallArray[i] = CornerWallID;
						}
						int b_Count = b_.startIndex + b_.wallCount;
						for(int i = b_.startIndex ; i < b_Count ; i++)
						{
							bc.wallArray[i] = EmptyWallID;
						}
					}
				}
				void MergeWallQuarterAndHalf(ref WallVariationRawData wall)
				{
					while(QuarterAndHalf(ref wall)) { }
					bool QuarterAndHalf(ref WallVariationRawData wall)
					{
						var abWall = wall.GetWallList();
						int length = abWall.Count;
						for(int i = 0 ; i < length ; i++)
						{
							(int wallID, int startIndex, int wallCount) = abWall[i];
							if(wallID<0) continue;
							if(wallCount == BlockSize) continue;

							int mergeWallCount = wallCount;

							for(int ii = i+1 ; ii < length ; ii++)
							{
								(int wallID, int startIndex, int wallCount)next = abWall[ii];
								if(next.wallID < 0) { break; }
								if(next.wallCount == BlockSize) { break; }

								mergeWallCount += next.wallCount;
								if(mergeWallCount == HalfBlockSize)
								{
									int endCount = startIndex + mergeWallCount;
									for(int iii = startIndex ; iii < endCount ; iii++)
									{
										wall.wallArray[iii] = wallID;
									}
									return true;
								}
								else if(mergeWallCount == BlockSize)
								{
									int endCount = startIndex + mergeWallCount;
									for(int iii = startIndex ; iii < endCount ; iii++)
									{
										wall.wallArray[iii] = wallID;
									}
									return true;
								}
								else if(mergeWallCount > BlockSize)
								{
									// 병합 가능한 크기 초과
									break;
								}
							}
						}
						return false;
					}
				}


				wallVariationRawDataX.PrintWall();
				wallVariationRawDataY.PrintWall();
				wallVariationRawDataIX.PrintWall();
				wallVariationRawDataIY.PrintWall();
			}
			void PropVariation()
			{

			}
		}

		[ButtonGroup, Button(Name = "Start Instantiate")]
		public async Awaitable CreateRoomResources()
		{
			WorldMapSystem worldMapSystem = await ThisContainer.AwaitGetParentObject<WorldMapSystem>(null,DestroyCancelToken);
			if(worldMapSystem == null) return;

			var IResources = worldMapSystem.AppController.ResourcesController;
			if(IResources == null) return;
			RoomVariationData roomVariationData = ThisContainer.GetData<RoomVariationData>();
			if(roomVariationData == null) return;
			var roomResourcesData = await IResources.GetAsset<RoomResourcesData>(roomVariationData.roomResourcesDataKey);
			if(roomResourcesData == null) return;
			RoomTransformData roomTransformData = ThisContainer.GetData<RoomTransformData>();
			if(roomTransformData  == null) return;

			// 랜덤 수 사전 설정
			List<float> randomValue = new List<float>();
			int randomHandle = 0;
			SetRandomValue();
			//필요한 수 만큼의 랜덤값을 미리 추려둔다.(오버해도 되니 넉넉하게)
			void SetRandomValue()
			{
				var oldState = Random.state;
				Random.InitState(roomVariationData.roomRandomSeed);
				randomHandle = 0;
				// 바닥 에서 사용할 랜덤값 세팅
				Vector2Int roomFloorCount = floorVariationRawData.RoomFloorCount;
				int xLength = roomFloorCount.x;
				int yLength = roomFloorCount.y;
				int length = xLength * yLength;
				for(int i = 0 ; i < length ; i++)
				{
					randomValue.Add(Random.Range(0f, 1f));
				}
				// 벽 에서 사용할 랜덤값 세팅
				Vector2Int roomFloorSize = floorVariationRawData.RoomFloorSize;
				xLength = roomFloorSize.x;
				yLength = roomFloorSize.y;
				length = xLength * yLength * 2;
				for(int i = 0 ; i < length ; i++)
				{
					randomValue.Add(Random.Range(0f, 1f));
				}
				Random.state = oldState;
			}
			float GetRandomValue()
			{
				if(randomHandle >= randomValue.Count) randomHandle = 0;
				return randomValue[randomHandle++];
			}
			float RandomRangeFloat(float min, float max)
			{
				return (min + (max - min) * GetRandomValue());
			}
			int RandomRangeInt(int min, int max)
			{
				return (int)(min + (max - min) * GetRandomValue());
			}

			// 룸 리소스 생성
			List<GameObject> floorObjects = new List<GameObject>();
			List<GameObject> wallObjects = new List<GameObject>();
			List<GameObject> doorObjects = new List<GameObject>();
			await AwaitableUtility.WaitAll(FloorInstantiate(), WallInstantiate());
			async Awaitable FloorInstantiate()
			{
				Vector2Int modelSize = floorVariationRawData.FloorModelSize;
				Vector3 modelSizeV3 =  floorVariationRawData.FloorModelSize.XZ();
				Vector3 realModelSizeV3 = modelSizeV3 * ModelRealSize;

				var floorList = roomResourcesData.floorPrefab.Where(f => f.size == modelSize).ToArray();
				if(floorList.Length == 0) return;

				Vector2Int roomFloorCount = floorVariationRawData.RoomFloorCount;
				int xLength = roomFloorCount.x;
				int yLength = roomFloorCount.y;

				for(int x = 0 ; x < xLength ; x++)
				{
					for(int y = 0 ; y < yLength ; y++)
					{
						Vector3 position = new Vector3(realModelSizeV3.x*x, 0, realModelSizeV3.z*y);
						Quaternion rotation = Quaternion.AngleAxis(180f, Vector3.up);
						int randomIndex = RandomRangeInt(0, floorList.Length);
						var key = floorList[randomIndex].resourcesKey;
						var floor = await IResources.LocalInstantiate(key, position, rotation, roomTransformData.floorParent);
						if(floor == null)
						{
							Debug.LogError($"Floor Instantiate Fail:({x},{y})({key.Path})({key.LoadAPI})");
							continue;
						}
						floorObjects.Add(floor);
					}
				}
				return;
			}
			async Awaitable WallInstantiate()
			{
				Vector3 roomFloorSize =  floorVariationRawData.RoomFloorSize.XZ();
				Vector3 realModelSizeV3 = roomFloorSize * ModelRealSize;

				Dictionary<int, RoomResourcesData.WallResourcesData[]> sizeOfDoorPrefabs = new Dictionary<int, RoomResourcesData.WallResourcesData[]>();
				Dictionary<int, RoomResourcesData.WallResourcesData[]> sizeOfCornerPrefab = new Dictionary<int, RoomResourcesData.WallResourcesData[]>();
				Dictionary<int, RoomResourcesData.WallResourcesData[]> sizeOfWallPrefab = new Dictionary<int, RoomResourcesData.WallResourcesData[]>();


				await AwaitableUtility.WaitAll(
					//_WallVariation(default, default, default),
					_WallVariation(wallVariationRawDataX, new Vector3(realModelSizeV3.x, 0, 0), Vector3.forward),
					_WallVariation(wallVariationRawDataY, new Vector3(realModelSizeV3.x, 0, realModelSizeV3.z), Vector3.left),
					_WallVariation(wallVariationRawDataIX, new Vector3(0, 0, realModelSizeV3.z), Vector3.back),
					_WallVariation(wallVariationRawDataIY, new Vector3(0, 0, 0), Vector3.right));
				//await _WallVariation(wallVariationRawDataIY, new Vector3(0, 0, 0), Vector3.right);

				sizeOfDoorPrefabs.Clear();
				sizeOfCornerPrefab.Clear();
				sizeOfWallPrefab.Clear();

				async Awaitable _WallVariation(WallVariationRawData wallVariationRawData, Vector3 startPos, Vector3 direction)
				{
					var wallDataList = wallVariationRawData.GetWallList();
					int length = wallDataList.Count;
					for(int i = 0 ; i < length ; i++)
					{
						(int wallID, int startIndex, int wallSize) = wallDataList[i];
						if(wallID == EmptyWallID) continue;

						float realStartOffset = ModelRealSize * startIndex;
						Vector3 realStartPoint = startPos + direction * realStartOffset;
						Quaternion rotationY  = Quaternion.identity;

						RoomResourcesData.WallResourcesData[] wallResourcesList = null;
						if(wallID == DoorWallID)
						{
							if(!sizeOfDoorPrefabs.TryGetValue(wallSize, out var _wallResourcesList))
							{
								_wallResourcesList = roomResourcesData.doorPrefab.Where(f => f.size == wallSize).ToArray();
								sizeOfDoorPrefabs.Add(wallSize, _wallResourcesList);
							}
							wallResourcesList = _wallResourcesList;

							rotationY = Quaternion.LookRotation(Quaternion.AngleAxis(90, Vector3.up)  * direction, Vector3.up);
						}
						else if(wallID == CornerWallID)
						{
							if(!sizeOfCornerPrefab.TryGetValue(wallSize, out var _wallResourcesList))
							{
								_wallResourcesList = roomResourcesData.cornerPrefab.Where(f => f.size == wallSize).ToArray();
								sizeOfCornerPrefab.Add(wallSize, _wallResourcesList);
							}
							wallResourcesList = _wallResourcesList;

							rotationY = Quaternion.LookRotation(Quaternion.AngleAxis(0, Vector3.up)  * direction, Vector3.up);
						}
						else
						{
							if(!sizeOfWallPrefab.TryGetValue(wallSize, out var _wallResourcesList))
							{
								_wallResourcesList = roomResourcesData.wallPrefab.Where(f => f.size == wallSize).ToArray();
								sizeOfWallPrefab.Add(wallSize, _wallResourcesList);
							}
							wallResourcesList = _wallResourcesList;

							rotationY = Quaternion.LookRotation(Quaternion.AngleAxis(90, Vector3.up)  * direction, Vector3.up);
						}

						int randomIndex = RandomRangeInt(0, wallResourcesList.Length);
						var resourcesKey = wallResourcesList[randomIndex].resourcesKey;

						var wall = await IResources.LocalInstantiate(resourcesKey, realStartPoint, rotationY, roomTransformData.wallParent);

						(wallID == DoorWallID ? doorObjects : wallObjects).Add(wall);
					}
				}
			}

			if(defaultFloor != null)
			{
				defaultFloor.SetActive(false);
				defaultFloor = null;
			}

			// 룸 컴퍼넌트 할당
			floorObjects.ForEach(floor => {
				floor.gameObject.SetActive(false);

				RoomElement element = floor.AddComponent<RoomElement>();
				element.ThisContainer.AddComponent<RoomFloor>();

				floor.gameObject.SetActive(true);
			});

			// 룸 컴퍼넌트 할당
			wallObjects.ForEach(wall => {
				wall.gameObject.SetActive(false);

				RoomElement element = wall.AddComponent<RoomElement>();
				element.ThisContainer.AddComponent<RoomWall>();

				wall.gameObject.SetActive(true);
			});

			doorObjects.ForEach(door => {
				door.gameObject.SetActive(false);

				RoomElement element = door.AddComponent<RoomElement>();
				element.ThisContainer.AddComponent<RoomDoor>();
				element.ThisContainer.AddData<NodeLinkData>();

				door.gameObject.SetActive(true);
			});
		}
		[ButtonGroup, Button(Name = "Clear Instantiate")]
		public void ClearRoomResources()
		{
			RoomElement[] roomElements = GetComponentsInChildren<RoomElement>(true);

			int length = roomElements.Length;
			for(int i = 0 ; i < length ; i++)
			{
				var roomElement = roomElements[i];
				if(!roomElement.TryGetComponent<RoomProp>(out var prop))
				{
					roomElement.DestroyThis();
				}
			}
		}
		[ButtonGroup, Button(Name = "Create Prop")]
		public async Awaitable CreatePropResources()
		{
			await Awaitable.NextFrameAsync();
		}
		[ButtonGroup, Button(Name = "Clear Prop")]
		public void ClearPropResources()
		{
			RoomElement[] roomElements = GetComponentsInChildren<RoomElement>(true);

			int length = roomElements.Length;
			for(int i = 0 ; i < length ; i++)
			{
				var roomElement = roomElements[i];
				if(roomElement.TryGetComponent<RoomProp>(out var prop))
				{
					roomElement.DestroyThis();
				}
			}
		}
		public void ClearAllResources()
		{
			RoomElement[] roomElements = GetComponentsInChildren<RoomElement>(true);

			int length = roomElements.Length;
			for(int i = 0 ; i < length ; i++)
			{
				var roomElement = roomElements[i];
				roomElement.DestroyThis();
			}
		}
	}
}