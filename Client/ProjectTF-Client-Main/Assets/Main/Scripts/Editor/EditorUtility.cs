#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using UnityEngine;

using Object = UnityEngine.Object;

namespace TFEditor
{
	public static class EditorUtility
	{
		public static List<string> FindAddressableAssetsInGroupName(string groupName, Func<Object, bool> assetCondition)
		{
			List<string> assetPaths = new List<string>();
			if(assetCondition == null) return assetPaths;

			var settings = AddressableAssetSettingsDefaultObject.Settings;

			List<AddressableAssetEntry> assets = new List<AddressableAssetEntry>();
			if(string.IsNullOrWhiteSpace(groupName))
			{
				assets = new List<AddressableAssetEntry>();
				settings.GetAllAssets(assets, true);
			}
			else
			{
				AddressableAssetGroup group = settings.FindGroup(groupName);
				//
				//// 그룹이 없으면 경고 메시지
				if(group == null || group.entries == null)
				{
					Debug.LogError("Group not found: " + groupName);
					return assetPaths;
				}
				assets = group.entries.ToList();
			}

			GatherAllAssets(assets);
			void GatherAllAssets(List<AddressableAssetEntry> entries)
			{
				foreach(var entry in entries)
				{
					if(entry.IsFolder)
					{
						List<AddressableAssetEntry> assets = new List<AddressableAssetEntry>();
						entry.GatherAllAssets(assets, false, true, false);
						GatherAllAssets(assets);
					}
					else if(entry.IsScene)
					{

					}
					else if(entry.IsSubAsset)
					{

					}
					else
					{
						if(assetCondition(entry.TargetAsset))
						{
							assetPaths.Add(entry.address); // 프리팹 주소를 리스트에 추가
						}
					}
				}
			}

			// 가져온 프리팹 목록 출력
			Debug.Log("Prefabs in group " + groupName + ":");
			foreach(var prefabPath in assetPaths)
			{
				Debug.Log(prefabPath);
			}


			return assetPaths;
		}

		public static List<string> FindResourcesrAssetsInFolderName(string folderName, Func<Object, bool> assetCondition)
		{
			List<string> assetPaths = new List<string>();
			if(assetCondition == null) return assetPaths;

			Object[] allObjects = Resources.LoadAll(folderName, typeof(GameObject));
			foreach(Object obj in allObjects)
			{
				if(assetCondition(obj))
				{
					string path = AssetDatabase.GetAssetPath(obj);
					string relativePath = path.Substring(path.IndexOf("Resources") + "Resources".Length + 1);
					string pathWithoutExtension = relativePath.Substring(0, relativePath.LastIndexOf('.'));

					assetPaths.Add(pathWithoutExtension);
				}
			}
			return assetPaths;
		}
	}
}
#endif