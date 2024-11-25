using System;
using System.Collections.Generic;

using UnityEngine;

using BC.ODCC;

namespace TFContent
{
    public class CharacterItemBoxData : ObjectBehaviour
    {
        public class ItemBoxCellData
        {
            public int itemIdx;
            public List<Vector3Int> itemBoxCellDatas;
		}

		public Vector3Int currentSize;

	}
}
