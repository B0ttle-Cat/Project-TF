using System;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TFContent
{
	[Serializable]
	public struct WorldMapRawData : IDisposable
	{
		public Vector2Int mapSize;
		public RoomNodeData[] nodes;
		public RoomVariationData[] variantDatas;

		public bool Validity {
			get {
				int size = mapSize.x * mapSize.y;
				if(size <= 0) return false;
				if(nodes.Length != size) return false;
				if(variantDatas.Length != size) return false;
				return true;
			}
		}
		public WorldMapRawData(Vector2Int mapSize)
		{
			this.mapSize=mapSize;
			nodes = new RoomNodeData[mapSize.x * mapSize.y];
			variantDatas = new RoomVariationData[mapSize.x * mapSize.y];
		}

		[Serializable]
		public struct RoomNodeData
		{
			[Title("ThisNode ID")]
			public Vector2Int tableIndex;    // X Index -↙↗+  || Y Index +↖↘-
			public int nodeIndex;

			[TitleGroup("NextNode ID")]
			[HorizontalGroup("NextNode ID/1"),HideLabel, SuffixLabel("+Y Node (↖)", Overlay = true)]
			public int yNodeIndex; // + Y 방행	↖
			[HorizontalGroup("NextNode ID/1"),HideLabel, SuffixLabel("+X Node (↗)", Overlay = true)]
			public int xNodeIndex; // + X 방향	↗
			[HorizontalGroup("NextNode ID/2"),HideLabel, SuffixLabel("-X Node (↙)", Overlay = true)]
			public int iXNodeIndex; // - X 방향	↙
			[HorizontalGroup("NextNode ID/2"),HideLabel, SuffixLabel("-Y Node (↘)", Overlay = true)]
			public int iYNodeIndex; // - Y 방향	↘

			public int[] NeighborNodeToArray()
			{
				int[] neighborNodeArray = new int[4];
				neighborNodeArray[0] = xNodeIndex;
				neighborNodeArray[1] = yNodeIndex;
				neighborNodeArray[2] = iXNodeIndex;
				neighborNodeArray[3] = iYNodeIndex;
				return neighborNodeArray;
			}
		}
		[Serializable]
		public struct RoomVariationData
		{
			[TitleGroup("Variation")]
			[HorizontalGroup("Variation/1"), LabelText("Theme"), LabelWidth(50)]
			[ValueDropdown("FindAllRoomThemeTables_ValueDropdownList")]
			public string themeName;

			[HorizontalGroup("Variation/1"), LabelText("Variation"), LabelWidth(50)]
			[ValueDropdown("RoomContentType_ValueDropdownList")]
			public int contentType;

			[HorizontalGroup("Variation/1"), LabelText("RandomID"), LabelWidth(50)]
			public int randomSeed;

			public RoomContentType RoomContentType => (RoomContentType)contentType;
#if UNITY_EDITOR
			private ValueDropdownList<string> FindAllRoomThemeTables_ValueDropdownList() => RoomDefine.FindAllRoomThemeTables_ValueDropdownList();
			private ValueDropdownList<int> RoomContentType_ValueDropdownList()
			{
				ValueDropdownList<int>  valueDropdownList = new ValueDropdownList<int>();
				foreach(RoomContentType item in Enum.GetValues(typeof(RoomContentType)))
				{
					valueDropdownList.Add(item.ToString(), (int)item);
				}

				return valueDropdownList;
			}
#endif
		}

		public void Dispose()
		{
			nodes = null;
		}
	}
}
