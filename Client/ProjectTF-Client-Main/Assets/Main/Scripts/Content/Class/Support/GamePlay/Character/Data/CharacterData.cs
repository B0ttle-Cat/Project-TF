using System.Collections.Generic;

using UnityEngine;

using BC.ODCC;

namespace TFContent.Character
{
	public class CharacterData : DataObject
	{
		public int idx;
		public eCharacterType type;

		public Vector2Int itemBoxSize = new Vector2Int(5, 2);
		public List<ItemBoxData> itemBoxDatas;
		public class ItemBoxData
		{
			public int itemId;
			public List<Vector2Int> points;
		}

		public void SetData(CharacterData data)
		{
			this.idx = data.idx;
			this.type = data.type;
		}
	}
}