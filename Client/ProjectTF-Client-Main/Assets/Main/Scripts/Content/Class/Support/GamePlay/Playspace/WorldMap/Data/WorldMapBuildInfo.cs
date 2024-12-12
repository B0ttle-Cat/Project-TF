using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TFContent.Playspace
{
	public class WorldMapBuildInfo : DataObject
	{
		public string mapHaskey;

		[InlineProperty, HideLabel]
		public WorldMapRawData worldMapRawData;

		public WorldMapBuildInfo() : base()
		{

		}

		protected override void Disposing()
		{
			worldMapRawData.Dispose();
		}

		public static WorldMapRawData LocalCreateWorldMap(WorldMapUserSettingData mapUserSetting)
		{
			if(mapUserSetting == null) return default;

			int seed = mapUserSetting.mapSeed;
			float multipathRate = mapUserSetting.multipathRate;
			Vector2Int mapSize = mapUserSetting.mapSizeXZ;
			int width = mapSize.x;
			int height = mapSize.y;
			string themeName = mapUserSetting.roomThemeName;
			RoomContentCreatePoint roomContentCreatePoint = mapUserSetting.roomContentCreatePoint;

			var prevState = Random.state;
			Random.InitState(seed);

			WorldMapRawData mapData = CreateWorldMapRawData(width, height, mapSize, themeName);
			RunDFS(width, height, ref mapData);
			RandomAddNodeLink(width, height, multipathRate, ref mapData);
			RandomRoomContent(seed, roomContentCreatePoint, ref mapData);

			Random.state = prevState;
			return mapData;

			static WorldMapRawData CreateWorldMapRawData(int width, int height, Vector2Int mapSize, string themeName)
			{
				// 초기화: RoomData 생성
				WorldMapRawData mapData = new WorldMapRawData(mapSize);
				for(int y = 0 ; y < height ; y++)
				{
					for(int x = 0 ; x < width ; x++)
					{
						int nodeIndex = y * width + x;
						mapData.nodes[nodeIndex] = new WorldMapRawData.RoomNodeData {
							tableIndex = new Vector2Int(x, y),
							nodeIndex = nodeIndex,
							xNodeIndex = -1,
							yNodeIndex = -1,
							iXNodeIndex = -1,
							iYNodeIndex = -1,
						};
						mapData.variantDatas[nodeIndex] = new WorldMapRawData.RoomVariationData {
							themeName = themeName,
							contentType = (int)RoomContentType.일반방,
							randomSeed = -1,
						};
					}
				}
				return mapData;
			}
			static void RunDFS(int width, int height, ref WorldMapRawData mapData)
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
					var currentRoom = mapData.nodes[currentNode];

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
			static void RandomAddNodeLink(int width, int height, float multipathRate, ref WorldMapRawData mapData)
			{
				if(multipathRate < 0) return;
				else if(multipathRate > 1) multipathRate = 1;

				int arrayLength = mapData.nodes.Length;
				for(int i = 0 ; i < arrayLength ; i++)
				{
					var roomData = mapData.nodes[i];
					int x = roomData.tableIndex.x;
					int y = roomData.tableIndex.y;
					int nodeIndex = roomData.nodeIndex;

					List<(int, int)> neighbors = new List<(int, int)>();
					if(roomData.xNodeIndex < 0 && x + 1 < width) AddNeighbor(neighbors, nodeIndex + 1, 0);
					if(roomData.yNodeIndex < 0 && y + 1 < height) AddNeighbor(neighbors, nodeIndex + width, 1);

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
			static void RandomRoomContent(int seed, RoomContentCreatePoint roomContentCreatePoint, ref WorldMapRawData mapData)
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

				List<WorldMapRawData.RoomVariationData> roomCreateDataList = new List<WorldMapRawData.RoomVariationData>();
				int arrayLength = mapData.variantDatas.Length;
				for(int i = 0 ; i < arrayLength ; i++)
				{
					WorldMapRawData.RoomVariationData roomCreateData = new WorldMapRawData.RoomVariationData{
						themeName = mapData.variantDatas[i].themeName,
						contentType = (int)SelectRandomRoomContentType(),
						randomSeed = Random.Range(int.MinValue, int.MaxValue),
					};
					roomCreateDataList.Add(roomCreateData);
				}
				mapData.variantDatas = roomCreateDataList.OrderBy((_) => Random.Range(int.MinValue, int.MaxValue)).ToArray();


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
			static List<(int neighborIndex, int direction)> GetNeighbors(WorldMapRawData.RoomNodeData room, int width, int height, HashSet<int> visited)
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
			static void AddNeighbor(List<(int, int)> neighbors, int neighborIndex, int direction, HashSet<int> visited = null)
			{
				if(visited ==null || !visited.Contains(neighborIndex))
				{
					neighbors.Add((neighborIndex, direction));
				}
			}
			static void ConnectRooms(ref WorldMapRawData mapData, int from, int to, int direction)
			{
				// 연결 설정
				switch(direction)
				{
					case 0: // +X 방향 (↗)
						mapData.nodes[from].xNodeIndex = to;
						mapData.nodes[to].iXNodeIndex = from;
						break;
					case 1: // +Y 방향 (↖)
						mapData.nodes[from].yNodeIndex = to;
						mapData.nodes[to].iYNodeIndex = from;
						break;
					case 2: // -X 방향 (↙)
						mapData.nodes[from].iXNodeIndex = to;
						mapData.nodes[to].xNodeIndex = from;
						break;
					case 3: // -Y 방향 (↘)
						mapData.nodes[from].iYNodeIndex = to;
						mapData.nodes[to].yNodeIndex = from;
						break;
				}
			}
		}
#if UNITY_EDITOR
		public void Editor_DrawGizmos(Vector3 offset)
		{
			Gizmos.color = Color.yellow;
			List<(int,int)> isDrawLine = new List<(int,int)>();
			var roomNodeArray = worldMapRawData.nodes;
			if(roomNodeArray == null) return;
			foreach(var roomData in worldMapRawData.nodes)
			{
				Vector3 position = offset + new Vector3(roomData.tableIndex.x, .1f, roomData.tableIndex.y);
				Gizmos.DrawWireSphere(position, 0.1f);

				if(roomData.xNodeIndex >= 0)
				{
					var lineTarget = roomNodeArray[roomData.xNodeIndex];
					Vector3 position2 = offset +new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.yNodeIndex >= 0)
				{
					var lineTarget = roomNodeArray[roomData.yNodeIndex];
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
		}
#endif
	}
}