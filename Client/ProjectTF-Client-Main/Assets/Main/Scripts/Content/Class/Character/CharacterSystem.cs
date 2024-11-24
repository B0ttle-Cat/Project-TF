using UnityEngine;

using BC.Base;
using BC.ODCC;
using TF.System;

namespace TF.Content.Character
{
    public class CharacterSystem : SystemState
	{
		public override bool AwakeOnSystem()
		{
			ThisContainer.AddComponent<CharacterSearch>();
			ThisContainer.AddComponent<CharacterCreate>();
			ThisContainer.AddComponent<CharacterDelete>();
			return base.AwakeOnSystem();
		}
	}
}