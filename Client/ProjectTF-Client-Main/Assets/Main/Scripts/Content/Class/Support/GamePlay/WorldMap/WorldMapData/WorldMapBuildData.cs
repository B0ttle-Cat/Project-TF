using System;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;
namespace TFContent
{
	public class WorldMapBuildData : DataObject
	{
		public WorldMapBuildData() : base()
		{

		}

		//////////////////////////////
		[SerializeField, ValueDropdown("MapSizeList", AppendNextDrawer = true)]
		private Vector2Int mapSizeXZ;
		public Vector3Int MapSize { get => mapSizeXZ.XZ(); set => mapSizeXZ = value.XZ(); }
		public enum MapSizeType
		{
			Small = 8,
			Medium = 12,
			Large = 16,
		}
		//////////////////////////////

		//////////////////////////////
		[SerializeField, InlineButton("ChangeRandomSpeed", "Seed ", Icon = SdfIconType.Dice5)]
		private int mapSeed;
		public int MapSeed { get => mapSeed; set => mapSeed=value; }
		//////////////////////////////

		public ValueDropdownList<Vector2Int> MapSizeList()
		{
			ValueDropdownList<Vector2Int> valueDropdownItems = new ValueDropdownList<Vector2Int>();
			foreach(MapSizeType item in Enum.GetValues(typeof(MapSizeType)))
			{
				int size = (int)item;
				Vector2Int sizeVector = new Vector2Int(size, size);
				valueDropdownItems.Add(item.ToString(), sizeVector);
			}
			return valueDropdownItems;
		}
		public void ChangeRandomSpeed()
		{
			// 현재 시간을 기준으로 시드 생성
			mapSeed = Random.Range(int.MinValue, int.MaxValue);
		}
		protected override void Disposing()
		{

		}
	}
}