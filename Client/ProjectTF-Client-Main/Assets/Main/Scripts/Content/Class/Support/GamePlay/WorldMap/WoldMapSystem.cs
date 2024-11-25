using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class WoldMapSystem : SystemState
	{
		protected override void AwakeOnSystem()
		{
		}

		protected override void DestroyOnSystems()
		{
		}

		protected override async Awaitable StartWaitSystem()
		{
			await Awaitable.NextFrameAsync();
		}

		protected override async Awaitable EndedWaitSystem()
		{
			await Awaitable.NextFrameAsync();
		}
	}
}
