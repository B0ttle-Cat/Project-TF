using System;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
namespace TFContent.Playspace
{
	[Serializable]
	public struct LinkInfo
	{
		public int linkIndex;
		public NodeDir linkDir;
		public enum NodeDir
		{
			X_Dir,
			Y_Dir,
			iX_Dir,
			iY_Dir
		}
	}
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
	public class NodeLinkData : DataObject
	{
		public int nodeIndex;
		public Vector2Int tableIndex;
		[InlineProperty,HideLabel,Header("NodeLink")]
		public LinkInfo nodeLink;
	}
}