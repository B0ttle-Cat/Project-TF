using Sirenix.OdinInspector;

using TFSystem;

using UnityEngine;

namespace TFContent.Playspace
{
	public class WorldMapSystem : SystemState
	{
		[InlineProperty, HideLabel]
		public MapManageAPI mapManageAPI;


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

#if UNITY_EDITOR
		[ButtonGroup, Button(Name = "Editor Create Map")]
		void Editor_LocalCreateWorldMap()
		{
			if(!ThisContainer.TryGetData<WorldMapUserSettingData>(out var mapUserSetting)) return;
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;

			var worldMapRawData = WorldMapBuildInfo.LocalCreateWorldMap(mapUserSetting);
			mapBuildInfo.mapHaskey = "";
			mapBuildInfo.worldMapRawData = worldMapRawData;
		}
		[ButtonGroup, Button(Name = "Editor Upload Map")]
		async void Editor_UploadWorldMapRawData()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			mapBuildInfo.mapHaskey = await mapManageAPI.Editor_UploadWorldMapRawData(mapBuildInfo.worldMapRawData);
		}
		[ButtonGroup, Button(Name = "Editor Download Map")]
		async void Editor_DownloadWorldMapRawData()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			(WorldMapRawData data, string key) = await mapManageAPI.Editor_DownloadWorldMapRawData(mapBuildInfo.mapHaskey);
			mapBuildInfo.worldMapRawData = data;
			mapBuildInfo.mapHaskey = key;
		}
		[ButtonGroup, Button(Name = "Editor Get MapList")]
		async void Editor_GetWorldMapRawDataList()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			await mapManageAPI.Editor_GetWorldMapRawDataList();
		}
		public void OnDrawGizmos()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			mapBuildInfo.Editor_DrawGizmos(ThisTransform.position);
		}
#endif
	}
}