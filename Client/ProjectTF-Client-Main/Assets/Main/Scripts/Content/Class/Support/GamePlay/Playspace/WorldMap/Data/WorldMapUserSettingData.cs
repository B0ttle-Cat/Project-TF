using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;
namespace TFContent.Playspace
{
	public class WorldMapUserSettingData : DataObject
	{
		public WorldMapUserSettingData() : base()
		{

		}

		[InlineButton("ChangeRandomSpeed", "Seed ", Icon = SdfIconType.Dice5)]
		public int mapSeed;

		[ValueDropdown("MapSizeList", AppendNextDrawer = true)]
		public Vector2Int mapSizeXZ;

		[Range(0f,1f)]
		public float multipathRate;

		[ValueDropdown("FindAllRoomThemeTables_ValueDropdownList", AppendNextDrawer = true)]
		public string roomThemeName;


		public void ChangeRandomSpeed()
		{
			// 현재 시간을 기준으로 시드 생성
			mapSeed = Random.Range(int.MinValue, int.MaxValue);
		}
#if UNITY_EDITOR
		public ValueDropdownList<Vector2Int> MapSizeList()
		{
			return MapDefine.MapSizeList();
		}
		public static ValueDropdownList<string> FindAllRoomThemeTables_ValueDropdownList()
		{
			return RoomDefine.FindAllRoomThemeTables_ValueDropdownList();
		}
#endif
		protected override void Disposing()
		{

		}
	}
}