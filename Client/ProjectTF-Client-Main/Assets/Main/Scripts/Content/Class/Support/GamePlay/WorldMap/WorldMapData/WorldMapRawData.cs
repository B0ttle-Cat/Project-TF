using System;
using System.Collections.Generic;

using UnityEngine;

using Random = System.Random;

namespace TFContent
{
	[Serializable]
	public struct WorldMapRawData
	{
		public int seed;
		public Vector2Int mapSize;
		public RoomData[] roomArray;
		[Serializable]
		public struct RoomData
		{
			public Vector2Int tableIndex;    // X Index -↙↗+  || Y Index +↖↘-
			public int nodeIndex;

			public int XNodeIndex; // + X 방향	↗
			public int YNodeIndex; // + Y 방행	↖
			public int iXNodeIndex; // - X 방향	↙
			public int iYNodeIndex; // - Y 방향	↘
		}

		public static WorldMapRawData CreateSample(Vector2Int? mapSize = null, int? seed = null)
		{
			int _seed = seed??Guid.NewGuid().GetHashCode();
			Vector2Int _mapSize = mapSize ?? new Vector2Int(8,8);
			int width = _mapSize.x;
			int height = _mapSize.y;
			Random random = new Random(_seed);
			WorldMapRawData mapData = new WorldMapRawData
			{
				seed = _seed,
				mapSize = _mapSize,
				roomArray = new WorldMapRawData.RoomData[width * height]
			};

			// 초기화: RoomData 생성
			for(int y = 0 ; y < height ; y++)
			{
				for(int x = 0 ; x < width ; x++)
				{
					int nodeIndex = y * width + x;
					mapData.roomArray[nodeIndex] = new WorldMapRawData.RoomData {
						tableIndex = new Vector2Int(x, y),
						nodeIndex = nodeIndex,
						XNodeIndex = -1,
						YNodeIndex = -1,
						iXNodeIndex = -1,
						iYNodeIndex = -1
					};
				}
			}

			RunDFS(random, width, height, mapData);
			RunDFS(random, width, height, mapData, .75f);

			return mapData;
		}

		private static void RunDFS(Random random, int width, int height, WorldMapRawData mapData, float connectRate = 1f)
		{
			// DFS 기반 미로 생성
			Stack<int> stack = new Stack<int>();
			HashSet<int> visited = new HashSet<int>();

			// 시작점 선택
			int startNode = random.Next(0, width * height);
			stack.Push(startNode);
			visited.Add(startNode);

			while(stack.Count > 0)
			{
				int currentNode = stack.Peek();
				var currentRoom = mapData.roomArray[currentNode];

				// 이동 가능한 방향 찾기
				List<(int neighborIndex, int direction)> neighbors = GetNeighbors(currentRoom, width, height, visited);

				if(neighbors.Count > 0)
				{
					// 랜덤으로 다음 노드 선택
					var (nextNode, direction) = neighbors[random.Next(neighbors.Count)];

					// 연결 설정
					if((float)random.NextDouble() <= connectRate)
					{
						ConnectRooms(mapData, currentNode, nextNode, direction);
					}

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

		private static List<(int neighborIndex, int direction)> GetNeighbors(
			WorldMapRawData.RoomData room,
			int width,
			int height,
			HashSet<int> visited)
		{
			List<(int, int)> neighbors = new List<(int, int)>();

			// 4방향 탐색
			int x = room.tableIndex.x;
			int y = room.tableIndex.y;

			if(x + 1 < width) AddNeighbor(neighbors, room.nodeIndex + 1, 0, visited); // +X 방향 (↗)
			if(y + 1 < height) AddNeighbor(neighbors, room.nodeIndex + width, 1, visited); // +Y 방향 (↖)
			if(x - 1 >= 0) AddNeighbor(neighbors, room.nodeIndex - 1, 2, visited); // -X 방향 (↙)
			if(y - 1 >= 0) AddNeighbor(neighbors, room.nodeIndex - width, 3, visited); // -Y 방향 (↘)

			return neighbors;
		}

		private static void AddNeighbor(List<(int, int)> neighbors, int neighborIndex, int direction, HashSet<int> visited)
		{
			if(!visited.Contains(neighborIndex))
			{
				neighbors.Add((neighborIndex, direction));
			}
		}

		private static void ConnectRooms(WorldMapRawData mapData, int from, int to, int direction)
		{
			// 연결 설정
			switch(direction)
			{
				case 0: // +X 방향 (↗)
					mapData.roomArray[from].XNodeIndex = to;
					mapData.roomArray[to].iXNodeIndex = from;
					break;
				case 1: // +Y 방향 (↖)
					mapData.roomArray[from].YNodeIndex = to;
					mapData.roomArray[to].iYNodeIndex = from;
					break;
				case 2: // -X 방향 (↙)
					mapData.roomArray[from].iXNodeIndex = to;
					mapData.roomArray[to].XNodeIndex = from;
					break;
				case 3: // -Y 방향 (↘)
					mapData.roomArray[from].iYNodeIndex = to;
					mapData.roomArray[to].YNodeIndex = from;
					break;
			}
		}

		public void DrawGizmos()
		{
#if UNITY_EDITOR
			Gizmos.color = Color.yellow;
			List<(int,int)> isDrawLine = new List<(int,int)>();
			foreach(var roomData in roomArray)
			{
				Vector3 position = new Vector3(roomData.tableIndex.x, .1f, roomData.tableIndex.y);
				Gizmos.DrawWireSphere(position, 0.1f);

				if(roomData.XNodeIndex >= 0)
				{
					var lineTarget = roomArray[roomData.XNodeIndex];
					Vector3 position2 = new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.YNodeIndex >= 0)
				{
					var lineTarget = roomArray[roomData.YNodeIndex];
					Vector3 position2 = new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.iXNodeIndex >= 0)
				{
					var lineTarget = roomArray[roomData.iXNodeIndex];
					Vector3 position2 = new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.iYNodeIndex >= 0)
				{
					var lineTarget = roomArray[roomData.iYNodeIndex];
					Vector3 position2 = new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
			}
#endif
		}
	}
}
