using UnityEngine;

using BC.Base;
using BC.ODCC;

namespace TFContent.Character
{
	public class Character : ObjectBehaviour
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

		protected override void BaseDestroy()
		{
			base.BaseDestroy();

		}

		protected override void BaseStart()
		{
			base.BaseStart();

		}

		protected override void BaseAwake()
		{
			base.BaseAwake();
			ThisContainer.AddComponent<CharacterModel>();
		}
	}
}
