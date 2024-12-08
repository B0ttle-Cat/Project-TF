using BC.ODCC;

using UnityEngine;
namespace TFContent.Playspace
{
	public class RoomNodeData : DataObject
	{

		public int nodeIndex;
		public Vector2Int tableIndex;
		public LinkInfo[] linkList;

		public RoomNodeData() : base()
		{
			nodeIndex = -1;
			linkList = new LinkInfo[0];
		}

		protected override void Disposing()
		{
			linkList = new LinkInfo[0];
		}
	}
}