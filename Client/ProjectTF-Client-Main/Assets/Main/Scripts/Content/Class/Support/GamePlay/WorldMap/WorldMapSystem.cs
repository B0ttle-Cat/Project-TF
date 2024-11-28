using Sirenix.OdinInspector;

using TFSystem;

using UnityEngine;

namespace TFContent
{
	public class WorldMapSystem : SystemState
	{

		protected override void AwakeOnSystem()
		{
		}

		protected override void DestroyOnSystems()
		{
		}

		protected override async Awaitable StartWaitSystem()
		{
		}

		protected override async Awaitable EndedWaitSystem()
		{
		}

		[ShowInInspector]
		WorldMapRawData? worldMapRawData;
		[Button]
		void TestWorldMapRawData()
		{
			worldMapRawData = WorldMapRawData.CreateSample();
		}

		public void OnDrawGizmos()
		{
			if(worldMapRawData.HasValue)
			{
				worldMapRawData.Value.DrawGizmos();
			}
		}
	}
}
