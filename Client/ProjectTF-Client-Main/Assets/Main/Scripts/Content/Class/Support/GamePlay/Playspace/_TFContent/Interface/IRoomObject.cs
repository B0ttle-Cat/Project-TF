using BC.ODCC;

using UnityEngine;
namespace TFContent
{
	public interface IRoomObject : IOdccObject
	{
		public Awaitable CreateRoomResources();
		public void ClearRoomResources();

		public Awaitable CreatePropResources();
		public void ClearPropResources();
	}
}