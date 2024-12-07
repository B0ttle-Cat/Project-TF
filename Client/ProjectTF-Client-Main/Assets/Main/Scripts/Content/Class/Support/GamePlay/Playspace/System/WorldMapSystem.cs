using Sirenix.OdinInspector;

using TFSystem;

using UnityEngine;

namespace TFContent.Playspace
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

		[Button]
		void TestCreateWorldMapRawData()
		{
			if(!ThisContainer.TryGetData<WorldMapUserSettingData>(out var mapUserSetting)) return;
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;

			var worldMapRawData = WorldMapRawData.CreateSample(mapUserSetting);
			mapBuildInfo.worldMapRawData = worldMapRawData;
		}

		public void OnDrawGizmos()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			mapBuildInfo.worldMapRawData.DrawGizmos(ThisTransform.position);
		}
	}
}