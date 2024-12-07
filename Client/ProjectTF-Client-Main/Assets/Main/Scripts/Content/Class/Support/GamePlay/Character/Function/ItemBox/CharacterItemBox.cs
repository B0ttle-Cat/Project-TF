using System.Collections.Generic;

using UnityEngine;

using BC.ODCC;

namespace TFContent.Character
{
	public class CharacterItemBox : ObjectBehaviour
	{
		protected override void BaseAwake()
		{
			base.BaseAwake();
			ThisContainer.AddComponent<ItemBoxSearch>();
			ThisContainer.AddComponent<ItemBoxMove>();
			ThisContainer.AddData<CharacterItemBoxData>();
		}
	}
}