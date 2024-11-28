using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
namespace TFContent
{
	public class RoomBuildData : DataObject
	{
		public RoomBuildData() : base()
		{

		}

		[FilePath(ParentFolder = "Assets/Main/Resources")]
		public string ResourcesKey;

		public Vector2Int RoomSize;

		public RoomContentFlag roomContentFlag;
		public RoomNodeDirectionFlag roomNodeDirectionFlag;

		protected override void Disposing()
		{

		}
	}
}