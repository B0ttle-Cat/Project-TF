using System;

using BC.ODCC;

namespace TFContent
{
	public interface IWorldMapSystem : IOdccObject
	{
		void OnSetNeighborCreateDepth(int nodeDepth);
		void OnChangeCurrentNode(int nodeIndex, Action<IRoomObject> completeCurrentRoom);
		bool TryGetRoomObject(int nodeIndex, out IRoomObject findRoomObject);
	}
}