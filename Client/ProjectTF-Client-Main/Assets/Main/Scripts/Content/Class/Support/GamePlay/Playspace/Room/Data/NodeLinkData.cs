using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TFContent.Playspace
{
	public class NodeLinkData : DataObject
	{
		public int nodeIndex;
		public Vector2Int tableIndex;
		[InlineProperty,HideLabel,Header("NodeLink")]
		public LinkInfo nodeLink;
	}
}
