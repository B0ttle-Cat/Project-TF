using BC.ODCC;

using Sirenix.OdinInspector;
namespace TFContent.Playspace
{
	public class WorldMapUserSettingData : DataObject
	{
		public WorldMapUserSettingData() : base()
		{

		}

		[InlineProperty, HideLabel]
		public WorldMapCreateDataInfo worldMapCreateDataInfo;

		[InlineProperty,HideLabel]
		public RoomContentCreateData roomContentCreateData;
		protected override void Disposing()
		{

		}
	}
}