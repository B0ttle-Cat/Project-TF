using UnityEngine;

using BC.Base;
using BC.ODCC;

namespace TF.Content.Character
{
    public class CharacterService : ObjectBehaviour
	{
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
			ThisContainer.AddComponent<CharacterBuilder>();
			ThisContainer.AddComponent<CharacterSearch>();
			ThisContainer.AddComponent<CharacterCreate>();
			ThisContainer.AddComponent<CharacterDelete>();
		}
	}
}