using UnityEngine;

using BC.Base;
using BC.ODCC;

namespace TFContent.Character
{
	public abstract class Character : ObjectBehaviour
	{
		public void SetCharacterData(CharacterData characterData)
		{
			if (ThisContainer.TryGetData<CharacterData>(out var data))
			{
				data.SetData(characterData);
			}
			else
			{
				ThisContainer.AddData(characterData);
			}
		}

		protected override void BaseAwake()
		{
			base.BaseAwake();
			ThisContainer.AddComponent<CharacterModel>();
		}
	}
}
