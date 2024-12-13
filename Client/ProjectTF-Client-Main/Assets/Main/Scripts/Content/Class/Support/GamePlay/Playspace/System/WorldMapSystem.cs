using System;

using BC.Base;

using Sirenix.OdinInspector;

using TFSystem;

using UnityEngine;

namespace TFContent.Playspace
{
	public class WorldMapSystem : SystemState, IWorldMapSystem
	{
		public IWorldMapBuilder WorldMapBuilder { get; private set; }
		public IRoomObjectGroup RoomObjectGroup { get; private set; }

		protected override void AwakeOnSystem()
		{
			if(ThisContainer.TryGetComponent<WorldMapBuilder>(out var worldMapBuilder))
			{
				WorldMapBuilder = worldMapBuilder;
			}
			if(ThisContainer.TryGetComponent<WorldRoomObjectGroup>(out var worldRoomObjectGroup))
			{
				RoomObjectGroup = worldRoomObjectGroup;
			}
		}

		protected override void DestroyOnSystems()
		{
			WorldMapBuilder = null;
			RoomObjectGroup = null;
		}

		protected override async Awaitable StartWaitSystem()
		{
			if(WorldMapBuilder != null) await AwaitableUtility.WaitTrue(() => WorldMapBuilder.IsValidity);
			if(RoomObjectGroup != null) await AwaitableUtility.WaitTrue(() => RoomObjectGroup.ReadyCurrentRoom);
		}

		protected override async Awaitable EndedWaitSystem()
		{
			if(RoomObjectGroup != null)
				RoomObjectGroup.ClearAllCreateRoom();
		}

		void OnSetNeighborCreateDepth(int nodeDepth)
		{
		}
		void OnChangeCurrentNode(int nodeIndex, Action<IRoomObject> completeCurrentRoom)
		{
		}
		bool TryGetRoomObject(int nodeIndex, out IRoomObject findRoomObject)
		{
			findRoomObject = null;




			return findRoomObject != null;
		}
#if UNITY_EDITOR
		[ButtonGroup, Button(Name = "Editor Create Map")]
		void Editor_LocalCreateWorldMap()
		{
			if(!ThisContainer.TryGetData<WorldMapUserSettingData>(out var mapUserSetting)) return;
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			if(!ThisContainer.TryGetComponent<IWorldMapBuilder>(out var worldMapBuilder)) return;

			var worldMapRawData = worldMapBuilder.CreateWorldMapRawData();
			mapBuildInfo.mapHaskey = "";
			mapBuildInfo.worldMapRawData = worldMapRawData;
		}
		[ButtonGroup, Button(Name = "Editor Upload Map")]
		async void Editor_UploadWorldMapRawData()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			mapBuildInfo.mapHaskey = await MapRepositoryAPI.Editor_UploadWorldMapRawData(mapBuildInfo.worldMapRawData);
		}
		[ButtonGroup, Button(Name = "Editor Download Map")]
		async void Editor_DownloadWorldMapRawData()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			(WorldMapRawData data, string key) = await MapRepositoryAPI.Editor_DownloadWorldMapRawData(mapBuildInfo.mapHaskey);
			mapBuildInfo.worldMapRawData = data;
			mapBuildInfo.mapHaskey = key;
		}
		[ButtonGroup, Button(Name = "Editor Get MapList")]
		async void Editor_GetWorldMapRawDataList()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			await MapRepositoryAPI.Editor_GetWorldMapRawDataList();
		}
		public void OnDrawGizmos()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			mapBuildInfo.Editor_DrawGizmos(ThisTransform.position);
		}
#endif

		void IWorldMapSystem.OnSetNeighborCreateDepth(int nodeDepth) => OnSetNeighborCreateDepth(nodeDepth);
		void IWorldMapSystem.OnChangeCurrentNode(int nodeIndex, Action<IRoomObject> completeCurrentRoom) => OnChangeCurrentNode(nodeIndex, completeCurrentRoom);
		bool IWorldMapSystem.TryGetRoomObject(int nodeIndex, out IRoomObject findRoomObject) => TryGetRoomObject(nodeIndex, out findRoomObject);


	}
}