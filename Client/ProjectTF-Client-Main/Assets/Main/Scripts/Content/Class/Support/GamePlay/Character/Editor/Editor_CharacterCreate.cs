#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

using TFContent.Character;

namespace TFContent
{
	public class Editor_CharacterCreate : Editor_Button
	{
		public bool isMy = false;
		public int idx;

		protected override void OnClick()
		{
			CharacterSystem system = FindAnyObjectByType<CharacterSystem>();
			if (system != null)
			{
				if (system.ThisContainer.TryGetComponent<ICharacterCreate>(out var create))
				{
					if (isMy)
					{
						create.Create<MyCharacter>(idx, eCharacterType.Normal, Success, null);
					}
					else
					{
						create.Create<OtherCharacter>(idx, eCharacterType.Normal, Success, null);
					}
				}
			}

			void Success<T>(T character)
			{

			}
		}
	}
}
#endif