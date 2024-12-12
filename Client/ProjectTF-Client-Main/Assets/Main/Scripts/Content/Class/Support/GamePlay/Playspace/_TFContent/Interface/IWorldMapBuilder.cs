using System;
using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;
namespace TFContent
{
	public interface
		IWorldMapBuilder : IOdccComponent
	{
		bool IsValidity { get; }

		void SetWorldMapUserSettingData(WorldMapCreateDataInfo worldMapCreateDataInfo, RoomContentCreateData roomContentCreateData);
		WorldMapRawData CreateWorldMapRawData();
		void SetWorldMapRawData(WorldMapRawData worldMapRawData, string mapHash);


		Awaitable CreateRoom(int createNodeIndex, HashSet<int> createdRooms);
		Awaitable CreateRoom(int createNodeIndex, int createNeighborDepth, HashSet<int> createdRooms, bool parallelCreateNeighbor, Action<List<int>> createNeighborRoom);
		void CancelCreate();
	}
}