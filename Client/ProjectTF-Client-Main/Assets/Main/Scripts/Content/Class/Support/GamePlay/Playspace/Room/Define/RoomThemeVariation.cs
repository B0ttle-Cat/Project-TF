using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TFContent.Playspace
{
	[CreateAssetMenu(fileName = "RoomThemeVariation", menuName = "Scriptable Objects/RoomThemeVariation")]
	public class RoomThemeVariation : ScriptableObject
	{
		public string ThemeName => name;
#if UNITY_EDITOR
		[HideLabel, ShowInInspector, DisplayAsString(EnableRichText = true), EnableGUI, PropertyOrder(-99)]
		private string Editor_ThemeName => $"Theme Name: <size=15><b>{ThemeName}</b></size>";
#endif

		[Title("")]
		public List<Table> variationList = new List<Table>();

		[Serializable]
		public struct Table
		{
			[Title("Table Name"), HideLabel]
			public string variationName;
			[Title("MaterialTable"), HideLabel, InlineProperty]
			public MaterialTable materialTable;
		}


		[Serializable]
		public struct MaterialTable
		{
			[HorizontalGroup,LabelWidth(40)]
			public Material wall;
			[HorizontalGroup,LabelWidth(40)]
			public Material plane;
			[HorizontalGroup,LabelWidth(40)]
			public Material node;
		}
	}

#if UNITY_EDITOR
	public static partial class RoomDefine
	{
		public static ValueDropdownList<string> FindAllRoomThemeTables_ValueDropdownList()
		{
			ValueDropdownList<string> valueDropdownItems = new ValueDropdownList<string>();
			var roomVariationTables = FindAllRoomThemeVariation();

			foreach(RoomThemeVariation table in roomVariationTables)
			{
				valueDropdownItems.Add($"{table.ThemeName}", table.ThemeName);
			}

			return valueDropdownItems;
		}
		public static ValueDropdownList<string> FindAllRoomVariationTables_ValueDropdownList(string themeName)
		{
			ValueDropdownList<string> valueDropdownItems = new ValueDropdownList<string>();
			var roomVariationTables = FindAllRoomThemeVariation();

			foreach(RoomThemeVariation table in roomVariationTables)
			{
				if(themeName == table.ThemeName)
				{
					foreach(RoomThemeVariation.Table variation in table.variationList)
					{
						valueDropdownItems.Add($"{variation.variationName}", variation.variationName);
					}
				}
			}

			return valueDropdownItems;
		}
		public static List<RoomThemeVariation> FindAllRoomThemeVariation()
		{
			// 리스트 초기화
			List<RoomThemeVariation> results = new List<RoomThemeVariation>();

			// 모든 에셋 경로 가져오기
			string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(RoomThemeVariation)}");

			foreach(string guid in guids)
			{
				// 경로로 에셋 로드
				string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				RoomThemeVariation table = UnityEditor.AssetDatabase.LoadAssetAtPath<RoomThemeVariation>(assetPath);

				if(table != null)
				{
					results.Add(table);
				}
			}

			return results;
		}
	}
#endif
}
