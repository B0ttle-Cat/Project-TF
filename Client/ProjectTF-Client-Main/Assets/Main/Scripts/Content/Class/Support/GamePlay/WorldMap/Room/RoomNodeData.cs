using System.Collections.Generic;

using BC.ODCC;
namespace TFContent
{
	public class RoomNodeData : DataObject
	{
		public int nodeIndex;
		public List<int> linkNodeIndex;

		public RoomNodeData() : base()
		{
			nodeIndex = -1;
			linkNodeIndex = new List<int>();
		}

		protected override void Disposing()
		{
			linkNodeIndex.Clear();
		}
	}
}