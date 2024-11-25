using UnityEngine;

using BC.Base;
using BC.ODCC;
using TF.System;

namespace TF.Content.Character
{
    public class CharacterSystem : SystemState
	{

		protected override void DestroyOnSystems()
		{

		}

		protected override Awaitable EndedWaitSystem()
		{
			throw new global::System.NotImplementedException();
		}

		protected override Awaitable StartWaitSystem()
		{
			throw new global::System.NotImplementedException();
		}

		protected override void AwakeOnSystem()
		{
			ThisContainer.AddComponent<CharacterSearch>();
			ThisContainer.AddComponent<CharacterCreate>();
			ThisContainer.AddComponent<CharacterDelete>();
		}
	}
}