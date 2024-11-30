using System;
using System.Collections.Generic;

using BC.ODCC;
namespace TFContent.Playspace
{
	public class RoomNodeData : DataObject
	{
		[Serializable]
		public struct LinkData
		{
			public int linkIndex;
			public NodeDir linkDir;
		}
		public enum NodeDir
		{
			X_Dir,
			Y_Dir,
			iX_Dir,
			iY_Dir
		}

		public int nodeIndex;
		public List<LinkData> linkNodeIndex;

		public RoomNodeData() : base()
		{
			nodeIndex = -1;
			linkNodeIndex = new List<LinkData>();
		}

		protected override void Disposing()
		{
			linkNodeIndex.Clear();
		}
	}
}