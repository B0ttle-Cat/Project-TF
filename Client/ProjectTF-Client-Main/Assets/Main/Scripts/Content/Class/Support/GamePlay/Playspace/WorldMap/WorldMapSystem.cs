using System;

using BC.Base;

using Sirenix.OdinInspector;

using TFSystem;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace TFContent.Playspace
{
	public class WorldMapSystem : SystemState, IWorldMapSystem
	{
		WorldRoomObjectGroup worldRoomObjectGroup;
		WorldMapBuilder worldMapBuilder;

		protected override void AwakeOnSystem()
		{

		}

		protected override void DestroyOnSystems()
		{
			worldRoomObjectGroup = null;
			worldMapBuilder = null;
		}

		protected override async Awaitable StartWaitSystem()
		{
			if(ThisContainer.TryGetComponent<WorldMapBuilder>(out worldMapBuilder))
			{
				Debug.Log("WorldMapSystem:Start:WorldMapBuilder");
				await AwaitableUtility.WaitTrue(() => worldMapBuilder.IsValidity);
				Debug.Log("WorldMapSystem:Ended:WorldMapBuilder");
			}
			if(ThisContainer.TryGetComponent<WorldRoomObjectGroup>(out worldRoomObjectGroup))
			{
				worldRoomObjectGroup.Builder = worldMapBuilder;
			}
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

		void IWorldMapSystem.OnSetNeighborCreateDepth(int nodeDepth)
		{
			if(!SystemIsReady) return;
			worldRoomObjectGroup.SetNeighborCreateDepth(nodeDepth);
		}

		void IWorldMapSystem.OnChangeCurrentNode(int nodeIndex, Action<IRoomObject> completeCurrentRoom)
		{
			if(!SystemIsReady) return;
			worldRoomObjectGroup.ChangeCurrentNode(nodeIndex, completeCurrentRoom);
		}

		bool IWorldMapSystem.TryGetRoomObject(int nodeIndex, out IRoomObject findRoomObject)
		{
			findRoomObject = null;
			if(!SystemIsReady) return false;

			return worldRoomObjectGroup.TryGetRoomObject(nodeIndex, out findRoomObject);
		}
	}
}