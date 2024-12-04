using System;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TFContent.Playspace
{
	public enum MapSizeType
	{
		Small = 8,
		Medium = 12,
		Large = 16,
	}

	public static partial class MapDefine
	{
#if UNITY_EDITOR
		public static ValueDropdownList<Vector2Int> MapSizeList()
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
#endif
	}
}
