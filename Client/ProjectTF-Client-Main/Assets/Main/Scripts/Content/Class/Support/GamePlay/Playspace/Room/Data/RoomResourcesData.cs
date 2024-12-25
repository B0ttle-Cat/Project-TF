using System;

using Sirenix.OdinInspector;

using UnityEngine;

using static TFSystem.IResourcesController;

namespace TFContent.Playspace
{
	[CreateAssetMenu(fileName = "RoomResourcesData", menuName = "Scriptable Objects/RoomResourcesData")]
	public class RoomResourcesData : ScriptableObject
	{

		[Serializable]
		public struct FloorResourcesData
		{
			[InlineProperty,HideLabel]
			[FoldoutGroup("@FoldoutGroupName")]
			public ResourcesKey resourcesKey;
			[FoldoutGroup("@FoldoutGroupName")]
			public Vector2Int size;
#if UNITY_EDITOR
			private string FoldoutGroupName => $"{resourcesKey.LoadAPI} | {size} | {resourcesKey.Path}";
#endif
		}
		[Serializable]
		public struct WallResourcesData
		{
			[InlineProperty,HideLabel]
			[FoldoutGroup("@FoldoutGroupName")]
			public ResourcesKey resourcesKey;
			[FoldoutGroup("@FoldoutGroupName")]
			public int size;

#if UNITY_EDITOR
			private string FoldoutGroupName => $"{resourcesKey.LoadAPI} | ({size}) | {resourcesKey.Path}";
#endif
		}
		[Header("Floor")]
		[ListDrawerSettings(NumberOfItemsPerPage = 10)]
		public FloorResourcesData[] floorPrefab;
		[Header("Wall")]
		[ListDrawerSettings(NumberOfItemsPerPage = 10)]
		public WallResourcesData[] wallPrefab;
		[ListDrawerSettings(NumberOfItemsPerPage = 10)]
		public WallResourcesData[] doorPrefab;
		[ListDrawerSettings(NumberOfItemsPerPage = 10)]
		public WallResourcesData[] cornerPrefab;
	}
}
