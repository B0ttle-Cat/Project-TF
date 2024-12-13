using System;
using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;
namespace TFContent
{
	public interface IRoomObjectGroup : IOdccComponent
	{
		public bool ReadyCurrentRoom { get; }

		int CreateNeighborDepth { get; set; }
		int CurrentRoomNodeIndex { get; }
		IRoomObject CurrentRoomObject { get; }

		Awaitable<IRoomObject> CreateRoom(int createNodeIndex, Action<List<IRoomObject>> createNeighborRoom);
		bool TryGetCreateRoom(int nodeIndex, out IRoomObject iRoomObject);
		void ClearAllCreateRoom();
	}
}