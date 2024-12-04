using System;
using System.Collections.Generic;
using System.Linq;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace TFContent.Playspace
{
	[Serializable]
	public struct WorldMapRawData : IDisposable
	{
		public int seed;
		public float multipathRate;
		public Vector2Int mapSize;

		public RoomNodeData[] roomNodeArray;
		public RoomVariationData[] roomVariationDataArray;

		public WorldMapRawData(Vector2Int mapSize, float multipathRate, int? seed = null)
		{
			this.seed= seed??Guid.NewGuid().GetHashCode();
			this.multipathRate = multipathRate;
			this.mapSize=mapSize;
			roomNodeArray = new RoomNodeData[mapSize.x * mapSize.y];
			roomVariationDataArray = new RoomVariationData[mapSize.x * mapSize.y];
		}

		[Serializable]
		public struct RoomNodeData
		{
			[Title("ThisNode ID")]
			public Vector2Int tableIndex;    // X Index -↙↗+  || Y Index +↖↘-
			public int nodeIndex;

			[TitleGroup("NextNode ID")]
			[HorizontalGroup("NextNode ID/1"),HideLabel, SuffixLabel("+Y Node (↖)", Overlay = true)]
			public int YNodeIndex; // + Y 방행	↖
			[HorizontalGroup("NextNode ID/1"),HideLabel, SuffixLabel("+X Node (↗)", Overlay = true)]
			public int XNodeIndex; // + X 방향	↗
			[HorizontalGroup("NextNode ID/2"),HideLabel, SuffixLabel("-X Node (↙)", Overlay = true)]
			public int iXNodeIndex; // - X 방향	↙
			[HorizontalGroup("NextNode ID/2"),HideLabel, SuffixLabel("-Y Node (↘)", Overlay = true)]
			public int iYNodeIndex; // - Y 방향	↘

			internal int[] NeighborNodeToArray()
			{
				int[] neighborNodeArray = new int[4];
				neighborNodeArray[0] = XNodeIndex;
				neighborNodeArray[1] = YNodeIndex;
				neighborNodeArray[2] = iXNodeIndex;
				neighborNodeArray[3] = iYNodeIndex;
				return neighborNodeArray;
			}
		}
		[Serializable]
		public struct RoomVariationData
		{
			[TitleGroup("Variation")]
			[HorizontalGroup("Variation/1"), LabelText("Theme"), LabelWidth(50)]
			[ValueDropdown("FindAllRoomThemeTables_ValueDropdownList")]
			public string roomThemeName;

			[HorizontalGroup("Variation/1"), LabelText("Variation"), LabelWidth(50)]
			[ValueDropdown("RoomContentType_ValueDropdownList")]
			public int roomContentType;

			[HorizontalGroup("Variation/1"), LabelText("RandomID"), LabelWidth(50)]
			public int roomRandomSeed;

			public RoomContentType RoomContentType => (RoomContentType)roomContentType;
#if UNITY_EDITOR
			private ValueDropdownList<string> FindAllRoomThemeTables_ValueDropdownList() => RoomDefine.FindAllRoomThemeTables_ValueDropdownList();
			private ValueDropdownList<int> RoomContentType_ValueDropdownList()
			{
				ValueDropdownList<int>  valueDropdownList = new ValueDropdownList<int>();
				foreach(RoomContentType item in Enum.GetValues(typeof(RoomContentType)))
				{
					valueDropdownList.Add(item.ToString(), (int)item);
				}

				return valueDropdownList;
			}
#endif
		}

		public static WorldMapRawData CreateSample(WorldMapUserSettingData mapUserSetting)
		{
			if(mapUserSetting == null) return default;

			int seed = mapUserSetting.mapSeed;
			float multipathRate = mapUserSetting.multipathRate;
			Vector2Int mapSize = mapUserSetting.mapSizeXZ;
			string themeName = mapUserSetting.roomThemeName;
			RoomContentCreatePoint roomContentCreatePoint = mapUserSetting.roomContentCreatePoint;

			var prevState = Random.state;
			Random.InitState(seed);

			int width = mapSize.x;
			int height = mapSize.y;
			WorldMapRawData mapData = new WorldMapRawData(mapSize , multipathRate, seed);

			// 초기화: RoomData 생성
			for(int y = 0 ; y < height ; y++)
			{
				for(int x = 0 ; x < width ; x++)
				{
					int nodeIndex = y * width + x;
					mapData.roomNodeArray[nodeIndex] = new RoomNodeData {
						tableIndex = new Vector2Int(x, y),
						nodeIndex = nodeIndex,
						XNodeIndex = -1,
						YNodeIndex = -1,
						iXNodeIndex = -1,
						iYNodeIndex = -1,
					};
					mapData.roomVariationDataArray[nodeIndex] = new RoomVariationData {
						roomThemeName = themeName,
						roomContentType = (int)RoomContentType.일반방,
						roomRandomSeed = -1,
					};
				}
			}

			RunDFS(width, height, ref mapData);
			RandomAddNodeLink(width, height, ref mapData);
			RandomRoomContent(seed, roomContentCreatePoint, ref mapData);

			Random.state = prevState;
			return mapData;
		}
		private static void RunDFS(int width, int height, ref WorldMapRawData mapData)
		{
			// DFS 기반 미로 생성
			Stack<int> stack = new Stack<int>();
			HashSet<int> visited = new HashSet<int>();

			// 시작점 선택
			int startNode = Random.Range(0, width * height);
			stack.Push(startNode);
			visited.Add(startNode);

			while(stack.Count > 0)
			{
				int currentNode = stack.Peek();
				var currentRoom = mapData.roomNodeArray[currentNode];

				// 이동 가능한 방향 찾기
				List<(int neighborIndex, int direction)> neighbors = GetNeighbors(currentRoom, width, height, visited);

				if(neighbors.Count > 0)
				{
					// 랜덤으로 다음 노드 선택
					var (nextNode, direction) = neighbors[Random.Range(0, neighbors.Count)];

					// 연결 설정
					ConnectRooms(ref mapData, currentNode, nextNode, direction);

					// 방문 처리
					visited.Add(nextNode);
					stack.Push(nextNode);
				}
				else
				{
					// 이동할 곳이 없으면 스택에서 제거
					stack.Pop();
				}
			}
		}
		private static void RandomAddNodeLink(int width, int height, ref WorldMapRawData mapData)
		{
			float multipathRate = mapData.multipathRate;
			if(multipathRate < 0) return;
			else if(multipathRate > 1) multipathRate = 1;

			int arrayLength = mapData.roomNodeArray.Length;
			for(int i = 0 ; i < arrayLength ; i++)
			{
				var roomData = mapData.roomNodeArray[i];
				int x = roomData.tableIndex.x;
				int y = roomData.tableIndex.y;
				int nodeIndex = roomData.nodeIndex;

				List<(int, int)> neighbors = new List<(int, int)>();
				if(roomData.XNodeIndex < 0 && x + 1 < width) AddNeighbor(neighbors, nodeIndex + 1, 0);
				if(roomData.YNodeIndex < 0 && y + 1 < height) AddNeighbor(neighbors, nodeIndex + width, 1);

				float neighborsLength = neighbors.Count;
				for(int ii = 0 ; ii < neighborsLength ; ii++)
				{
					if(Random.value <= multipathRate)
					{
						var (nextNode, direction) = neighbors[ii];
						ConnectRooms(ref mapData, nodeIndex, nextNode, direction);
					}
				}
			}

			//foreach(var item in extinctLinks)
			//{
			//	int ANode = item.A;
			//	int bNode = item.B;
			//	int Dir = item.Dir;
			//	if(Random.value <= multipathRate)
			//	{
			//		ConnectRooms(ref mapData, ANode, bNode, Dir);
			//	}
			//}
		}
		private static void RandomRoomContent(int seed, RoomContentCreatePoint roomContentCreatePoint, ref WorldMapRawData mapData)
		{
			int totalPoint = 0;
			List<RoomContentCreatePoint.ContentPoint> contentPointList = new List<RoomContentCreatePoint.ContentPoint>();
			int contentPointCount = roomContentCreatePoint.contentPoint.Count;
			for(int i = 0 ; i < contentPointCount ; i++)
			{
				RoomContentCreatePoint.ContentPoint contentPoint = roomContentCreatePoint.contentPoint[i];
				int findContentIndex =  contentPointList.FindIndex(x => x.contentType == contentPoint.contentType);
				if(findContentIndex < 0)
				{
					totalPoint += contentPoint.point;
					contentPoint.minCount = Mathf.Max(0, contentPoint.minCount);
					contentPointList.Add(contentPoint);
				}
				else
				{
					var findContentPoint = contentPointList[findContentIndex];
					totalPoint += contentPoint.point;
					findContentPoint.point += contentPoint.point;
					findContentPoint.minCount = Mathf.Max(findContentPoint.minCount, contentPoint.minCount);
					contentPointList[findContentIndex] = findContentPoint;
				}
			}
			if(totalPoint == 0) return;

			List<RoomVariationData> roomCreateDataList = new List<RoomVariationData>();
			int arrayLength = mapData.roomVariationDataArray.Length;
			for(int i = 0 ; i < arrayLength ; i++)
			{
				RoomVariationData roomCreateData = new RoomVariationData{
					roomThemeName = mapData.roomVariationDataArray[i].roomThemeName,
					roomContentType = (int)SelectRandomRoomContentType(),
					roomRandomSeed = Random.Range(int.MinValue, int.MaxValue),
				};
				roomCreateDataList.Add(roomCreateData);
			}
			mapData.roomVariationDataArray = roomCreateDataList.OrderBy((_) => Random.Range(int.MinValue, int.MaxValue)).ToArray();


			RoomContentType SelectRandomRoomContentType()
			{
				int contentPointCount = contentPointList.Count;
				for(int i = 0 ; i < contentPointCount ; i++)
				{
					var content = contentPointList[i];

					if(content.minCount > 0)
					{
						content.minCount--;
						contentPointList[i] = content;

						return content.contentType;
					}
				}

				int checkPoint = 0;
				int randomValue = Random.Range(0, totalPoint);
				for(int i = 0 ; i < contentPointCount ; i++)
				{
					var content = contentPointList[i];
					checkPoint += content.point;
					if(randomValue < checkPoint)
					{
						return content.contentType;
					}
				}
				return RoomContentType.일반방;
			}
		}
		private static List<(int neighborIndex, int direction)> GetNeighbors(WorldMapRawData.RoomNodeData room, int width, int height, HashSet<int> visited)
		{
			List<(int, int)> neighbors = new List<(int, int)>();

			// 4방향 탐색
			int x = room.tableIndex.x;
			int y = room.tableIndex.y;
			int nodeIndex = room.nodeIndex;

			if(x + 1 < width) AddNeighbor(neighbors, nodeIndex + 1, 0, visited); // +X 방향 (↗)
			if(y + 1 < height) AddNeighbor(neighbors, nodeIndex + width, 1, visited); // +Y 방향 (↖)
			if(x - 1 >= 0) AddNeighbor(neighbors, nodeIndex - 1, 2, visited); // -X 방향 (↙)
			if(y - 1 >= 0) AddNeighbor(neighbors, nodeIndex - width, 3, visited); // -Y 방향 (↘)

			return neighbors;
		}
		private static void AddNeighbor(List<(int, int)> neighbors, int neighborIndex, int direction, HashSet<int> visited = null)
		{
			if(visited ==null || !visited.Contains(neighborIndex))
			{
				neighbors.Add((neighborIndex, direction));
			}
		}
		private static void ConnectRooms(ref WorldMapRawData mapData, int from, int to, int direction)
		{
			// 연결 설정
			switch(direction)
			{
				case 0: // +X 방향 (↗)
					mapData.roomNodeArray[from].XNodeIndex = to;
					mapData.roomNodeArray[to].iXNodeIndex = from;
					break;
				case 1: // +Y 방향 (↖)
					mapData.roomNodeArray[from].YNodeIndex = to;
					mapData.roomNodeArray[to].iYNodeIndex = from;
					break;
				case 2: // -X 방향 (↙)
					mapData.roomNodeArray[from].iXNodeIndex = to;
					mapData.roomNodeArray[to].XNodeIndex = from;
					break;
				case 3: // -Y 방향 (↘)
					mapData.roomNodeArray[from].iYNodeIndex = to;
					mapData.roomNodeArray[to].YNodeIndex = from;
					break;
			}
		}
		public void DrawGizmos(Vector3 offset)
		{
#if UNITY_EDITOR
			Gizmos.color = Color.yellow;
			List<(int,int)> isDrawLine = new List<(int,int)>();
			foreach(var roomData in roomNodeArray)
			{
				Vector3 position = offset + new Vector3(roomData.tableIndex.x, .1f, roomData.tableIndex.y);
				Gizmos.DrawWireSphere(position, 0.1f);

				if(roomData.XNodeIndex >= 0)
				{
					var lineTarget = roomNodeArray[roomData.XNodeIndex];
					Vector3 position2 = offset +new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.YNodeIndex >= 0)
				{
					var lineTarget = roomNodeArray[roomData.YNodeIndex];
					Vector3 position2 = offset +new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.iXNodeIndex >= 0)
				{
					var lineTarget = roomNodeArray[roomData.iXNodeIndex];
					Vector3 position2 = offset +new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.iYNodeIndex >= 0)
				{
					var lineTarget = roomNodeArray[roomData.iYNodeIndex];
					Vector3 position2 = offset +new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
			}
#endif
		}
		public void Dispose()
		{
			roomNodeArray = null;
		}
	}
}
