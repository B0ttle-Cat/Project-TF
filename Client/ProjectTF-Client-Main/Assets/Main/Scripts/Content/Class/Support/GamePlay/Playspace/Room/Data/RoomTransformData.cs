using BC.ODCC;

using UnityEngine;
namespace TFContent.Playspace
{
	public class RoomTransformData : DataObject
	{
		public RoomTransformData() : base()
		{

		}

		public Transform roomTransform;
		[Space]
		public Transform floorParent;
		public Transform doorParent;
		public Transform wallParent;

		protected override void Disposing()
		{

		}
	}
}