using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TFContent.Playspace
{
	public class WorldMapBuildInfo : DataObject
	{
		[InlineProperty,HideLabel,Header("WorldMap Raw Data")]
		public WorldMapRawData worldMapRawData;

		public WorldMapBuildInfo() : base()
		{

		}

		protected override void Disposing()
		{
			worldMapRawData.Dispose();
		}
	}
}