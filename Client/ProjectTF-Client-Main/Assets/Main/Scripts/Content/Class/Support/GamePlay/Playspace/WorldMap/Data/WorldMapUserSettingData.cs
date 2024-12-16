using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
namespace TFContent.Playspace
{
	public class WorldMapUserSettingData : DataObject
	{
		public WorldMapUserSettingData() : base()
		{

		}

		[InlineProperty, HideLabel]
		[Header(nameof(WorldMapCreateDataInfo))]
		public WorldMapCreateDataInfo worldMapCreateDataInfo;

		[InlineProperty,HideLabel]
		[Header(nameof(RoomContentCreateData))]
		public RoomContentCreateData roomContentCreateData;
		protected override void Disposing()
		{

		}
	}
}