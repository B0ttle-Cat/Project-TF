using System;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace TFContent.Playspace
{
	[Serializable]
	public struct FloorVariationRawData
	{
		[ShowInInspector, ReadOnly]
		private Vector2Int floorModelSize;
		public Vector2Int roomFloorIndex;
		public Vector2Int roomFloorSize;

		public FloorVariationRawData(Vector2Int floorModelSize)
		{
			this.floorModelSize=floorModelSize;
			roomFloorIndex = Vector2Int.zero;
			roomFloorSize = Vector2Int.zero;
		}

		public void SetRandomFloor(int minInclusive, int maxExclusive)
		{
			roomFloorIndex.Set(Random.Range(minInclusive, maxExclusive), Random.Range(minInclusive, maxExclusive));
			roomFloorSize.Set(roomFloorIndex.x * floorModelSize.x, roomFloorIndex.y * floorModelSize.y);
		}
	}
}
