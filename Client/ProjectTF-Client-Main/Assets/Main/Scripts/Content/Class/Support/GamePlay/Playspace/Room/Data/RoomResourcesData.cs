using System;
using System.Collections.Generic;

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
			[InlineProperty,Header("ResourcesKey"),HideLabel]
			public ResourcesKey resourcesKey;
			[Header("Info")]
			public Vector2Int size;
		}
		[Serializable]
		public struct WallResourcesData
		{
#if UNITY_EDITOR
			private List<string> AddressablePathList()
			{
				var pathList = TFEditor.EditorUtility.FindAddressableAssetsInGroupName("Room Assets Group",asset => asset is GameObject);
				return pathList;
			}
			private List<string> ResourcesPathPathList()
			{
				var pathList = TFEditor.EditorUtility.FindResourcesrAssetsInFolderName("Prefabs",asset => asset is GameObject);
				return pathList;
			}
			private bool EditorIsAddressableAPI => resourcesKey.LoadAPI == AssetLoadAPI.AddressableAPI;
			[Button(Name = "Set Resources Key", ButtonHeight = (int)ButtonSizes.Medium, Style = ButtonStyle.Box), PropertyOrder(-10)]
			private void SetResourcesKey(
				[ShowIf("EditorIsAddressableAPI")][HideLabel][ValueDropdown("AddressablePathList")]
				string addressablePath,
				[HideIf("EditorIsAddressableAPI")][HideLabel][ValueDropdown("ResourcesPathPathList")]
				string resourcesPath)
			{
				resourcesKey = EditorIsAddressableAPI
					? new ResourcesKey(addressablePath, AssetLoadAPI.AddressableAPI)
					: new ResourcesKey(resourcesPath, AssetLoadAPI.ResourcesAPI);
			}
#endif

			[InlineProperty,Header("ResourcesKey"),HideLabel]
			public ResourcesKey resourcesKey;
			[Header("Info")]
			public int size;
		}
		public FloorResourcesData[] floorPrefab;
		public WallResourcesData[] wallPrefab;
		public WallResourcesData[] doorPrefab;
	}
}
