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
		public RoomContentType roomContentType;
		public int roomRandomSeed;

		protected override void Disposing()
		{

		}
#if UNITY_EDITOR
		private ValueDropdownList<string> FindAllRoomThemeTables_ValueDropdownList() => RoomDefine.FindAllRoomThemeTables_ValueDropdownList();
#endif
	}
}