﻿using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

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
		public RoomData[] roomArray;

		public WorldMapRawData(Vector2Int mapSize, float multipathRate, int? seed = null)
		{
			this.seed= seed??Guid.NewGuid().GetHashCode();
			this.multipathRate = multipathRate;
			this.mapSize=mapSize;
			this.roomArray=new RoomData[mapSize.x * mapSize.y];
		}

		[Serializable]
		public struct RoomData
		{
			[Title("ThisNode ID")]
			public Vector2Int tableIndex;    // X Index -↙↗+  || Y Index +↖↘-
			public int nodeIndex;

			[TitleGroup("NextNode ID")]
			[HorizontalGroup("NextNode ID/Node1"),HideLabel, SuffixLabel("+Y Node (↖)", Overlay = true)]
			public int YNodeIndex; // + Y 방행	↖
			[HorizontalGroup("NextNode ID/Node1"),HideLabel, SuffixLabel("+X Node (↗)", Overlay = true)]
			public int XNodeIndex; // + X 방향	↗
			[HorizontalGroup("NextNode ID/Node2"),HideLabel, SuffixLabel("-X Node (↙)", Overlay = true)]
			public int iXNodeIndex; // - X 방향	↙
			[HorizontalGroup("NextNode ID/Node2"),HideLabel, SuffixLabel("-Y Node (↘)", Overlay = true)]
			public int iYNodeIndex; // - Y 방향	↘
		}

		public static WorldMapRawData CreateSample(Vector2Int? mapSize = null, float? multipathRate = null, int? seed = null)
		{
			int _seed = seed??Guid.NewGuid().GetHashCode();
			float _multipathRate = multipathRate ?? 0.4f;
			Vector2Int _mapSize = mapSize ?? new Vector2Int(8,8);
			int width = _mapSize.x;
			int height = _mapSize.y;

			var prevState = Random.state;
			Random.InitState(_seed);

			WorldMapRawData mapData = new WorldMapRawData(_mapSize , _multipathRate, _seed);

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

			RunDFS(width, height, ref mapData);
			RandomAddNodeLink(width, height, ref mapData);

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
				var currentRoom = mapData.roomArray[currentNode];

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

			HashSet<(int A,int B, int Dir)> extinctLinks = new HashSet<(int, int, int)>();
			mapData.roomArray.ForEach(roomData => {
				int x = roomData.tableIndex.x;
				int y = roomData.tableIndex.y;
				int nodeIndex = roomData.nodeIndex;
				if(roomData.XNodeIndex < 0 && x + 1 < width) extinctLinks.Add(LinkID(nodeIndex, nodeIndex + 1, 0));
				if(roomData.YNodeIndex < 0 && y + 1 < height) extinctLinks.Add(LinkID(nodeIndex, nodeIndex + width, 1));
				//if(roomData.iXNodeIndex < 0 && x - 1 >= 0) extinctLinks.Add(LinkID(nodeIndex, nodeIndex - 1, 2));
				//if(roomData.iYNodeIndex < 0 && y - 1 >= 0) extinctLinks.Add(LinkID(nodeIndex, nodeIndex - width, 3));
			});
			(int, int, int) LinkID(int a, int b, int dir)
			{
				return a < b ? (a, b, dir) : (b, a, dir);
			}

			foreach(var item in extinctLinks)
			{
				int ANode = item.A;
				int bNode = item.B;
				int Dir = item.Dir;
				if(Random.value <= multipathRate)
				{
					ConnectRooms(ref mapData, ANode, bNode, Dir);
				}
			}
		}
		private static List<(int neighborIndex, int direction)> GetNeighbors(WorldMapRawData.RoomData room, int width, int height, HashSet<int> visited)
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
		private static void AddNeighbor(List<(int, int)> neighbors, int neighborIndex, int direction, HashSet<int> visited)
		{
			if(!visited.Contains(neighborIndex))
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
		public void DrawGizmos(Vector3 offset)
		{
#if UNITY_EDITOR
			Gizmos.color = Color.yellow;
			List<(int,int)> isDrawLine = new List<(int,int)>();
			foreach(var roomData in roomArray)
			{
				Vector3 position = offset + new Vector3(roomData.tableIndex.x, .1f, roomData.tableIndex.y);
				Gizmos.DrawWireSphere(position, 0.1f);

				if(roomData.XNodeIndex >= 0)
				{
					var lineTarget = roomArray[roomData.XNodeIndex];
					Vector3 position2 = offset +new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.YNodeIndex >= 0)
				{
					var lineTarget = roomArray[roomData.YNodeIndex];
					Vector3 position2 = offset +new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.iXNodeIndex >= 0)
				{
					var lineTarget = roomArray[roomData.iXNodeIndex];
					Vector3 position2 = offset +new Vector3(lineTarget.tableIndex.x, .1f, lineTarget.tableIndex.y);
					Gizmos.DrawLineList(new Vector3[] {
						position,
						position2,
					});

				}
				if(roomData.iYNodeIndex >= 0)
				{
					var lineTarget = roomArray[roomData.iYNodeIndex];
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
			roomArray = null;
		}
	}
}