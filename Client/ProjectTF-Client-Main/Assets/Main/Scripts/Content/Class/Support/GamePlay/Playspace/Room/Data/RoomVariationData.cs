using BC.ODCC;

using Sirenix.OdinInspector;
namespace TFContent.Playspace
{
	public class RoomVariationData : DataObject
	{
		public RoomVariationData() : base()
		{

		}

		[ValueDropdown("FindAllRoomThemeTables_ValueDropdownList")]
		public string roomThemeName;

		[ValueDropdown("FindAllRoomVariationTables_ValueDropdownList")]
		public string roomVariationName;

		public RoomContentFlag roomContentFlag;
		public RoomNodeDirectionFlag roomNodeDirectionFlag;

		protected override void Disposing()
		{

		}
#if UNITY_EDITOR
		public ValueDropdownList<string> FindAllRoomThemeTables_ValueDropdownList()
		{
			return RoomDefine.FindAllRoomThemeTables_ValueDropdownList();
		}
		public ValueDropdownList<string> FindAllRoomVariationTables_ValueDropdownList()
		{
			return RoomDefine.FindAllRoomVariationTables_ValueDropdownList(roomThemeName);
		}
#endif
	}
}