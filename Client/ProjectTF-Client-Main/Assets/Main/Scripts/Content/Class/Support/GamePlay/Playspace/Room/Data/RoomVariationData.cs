using BC.ODCC;

using Sirenix.OdinInspector;

using TFSystem;

using UnityEngine;
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
		[InlineProperty,HideLabel,Header("RoomResourcesDataKey")]
		public IResourcesController.ResourcesKey roomResourcesDataKey;

		protected override void Disposing()
		{

		}
#if UNITY_EDITOR
		private ValueDropdownList<string> FindAllRoomThemeTables_ValueDropdownList() => RoomDefine.FindAllRoomThemeTables_ValueDropdownList();
#endif
	}
}