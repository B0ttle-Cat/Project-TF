using System;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace TFContent.Playspace
{
	[Serializable]
	public struct FloorVariationRawData
	{
		[SerializeField, ReadOnly]
		private Vector2Int floorModelSize;
		[SerializeField]
		private Vector2Int roomFloorCount;
		[SerializeField]
		public Vector2Int roomFloorSize;
		// 리소스 모델의 크디
		public Vector2Int FloorModelSize { get => floorModelSize; private set => floorModelSize=value; }
		// 방에 몇개의 바닥이 있는지?
		public Vector2Int RoomFloorCount { get => roomFloorCount; private set => roomFloorCount=value; }
		// 방에의 전체 크기 (모델 크기 * 개수) 
		public Vector2Int RoomFloorSize { get => roomFloorSize; private set => roomFloorSize = value; }

		public FloorVariationRawData(Vector2Int modelSize)
		{
			floorModelSize = modelSize;
			roomFloorCount = Vector2Int.zero;
			roomFloorSize = Vector2Int.zero;
		}

		public void SetFloorRandomCount(int minInclusive, int maxExclusive)
		{
			roomFloorCount.Set(Random.Range(minInclusive, maxExclusive), Random.Range(minInclusive, maxExclusive));
			roomFloorSize.Set(roomFloorCount.x * floorModelSize.x, roomFloorCount.y * floorModelSize.y);
		}
		public void SetFloorCount(Vector2Int floorCount)
		{
			roomFloorCount = floorCount;
			roomFloorSize.Set(roomFloorCount.x * floorModelSize.x, roomFloorCount.y * floorModelSize.y);
		}
	}
}
