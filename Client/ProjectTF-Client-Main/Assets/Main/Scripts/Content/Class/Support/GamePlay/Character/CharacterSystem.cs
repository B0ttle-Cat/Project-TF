using TFSystem;

using UnityEngine;

namespace TFContent.Character
{
	public class CharacterSystem : SystemState
	{

		protected override async Awaitable EndedWaitSystem()
		{
			await Awaitable.NextFrameAsync();
		}

		protected override async Awaitable StartWaitSystem()
		{
			await Awaitable.NextFrameAsync();
		}

		protected override void DestroyOnSystems()
		{

		}

		protected override void AwakeOnSystem()
		{
			ThisContainer.AddComponent<CharacterSearch>();
			ThisContainer.AddComponent<CharacterCreate>();
			ThisContainer.AddComponent<CharacterDelete>();
		}
	}
}