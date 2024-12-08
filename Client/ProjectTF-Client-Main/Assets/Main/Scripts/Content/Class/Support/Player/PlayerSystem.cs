using UnityEngine;

using BC.ODCC;
using TFSystem;

namespace TFContent.Player
{
	public class PlayerSystem : SystemState
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
			ThisContainer.AddComponent<AddPlayer>();
			ThisContainer.AddComponent<DeletePlayer>();
			ThisContainer.AddComponent<SearchPlayer>();
		}
	}
}