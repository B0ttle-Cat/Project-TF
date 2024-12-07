#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

using TFContent.Character;

namespace TFContent
{
	public class Editor_ItemBoxCreate : Editor_Button
	{
		protected override void OnClick()
		{
			CharacterSystem system = FindAnyObjectByType<CharacterSystem>();
			if (system != null)
			{
				if (system.ThisContainer.TryGetComponent<ICharacterSearch>(out var search))
				{
					if (search.Search_ItemBox(out var itemBox))
					{
						if (itemBox.ThisContainer.TryGetComponent<ItemBoxBuilder>(out var builder))
						{
							builder.Create(Vector2Int.zero, new Vector2Int(5, 2));
						}
					}
				}
			}
		}
	}
}
#endif