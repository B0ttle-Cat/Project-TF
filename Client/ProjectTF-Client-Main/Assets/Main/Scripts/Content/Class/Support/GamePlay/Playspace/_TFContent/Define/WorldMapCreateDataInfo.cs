using System;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace TFContent
{
	[Serializable]
	public struct WorldMapCreateDataInfo
	{
		[InlineButton("ChangeRandomSpeed", "Seed ", Icon = SdfIconType.Dice5)]
		public int mapSeed;

		[ValueDropdown("MapSizeList", AppendNextDrawer = true)]
		public Vector2Int mapSizeXZ;

		[Range(0f,1f)]
		public float multipathRate;


		public void ChangeRandomSpeed()
		{
			// 현재 시간을 기준으로 시드 생성
			mapSeed = Random.Range(int.MinValue, int.MaxValue);
		}
#if UNITY_EDITOR
		private ValueDropdownList<Vector2Int> MapSizeList() => Playspace.MapDefine.MapSizeList();
		private ValueDropdownList<string> FindAllRoomThemeTables_ValueDropdownList() => RoomDefine.FindAllRoomThemeTables_ValueDropdownList();
#endif
	}
}
