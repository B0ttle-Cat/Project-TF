using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class IntroSystem : SystemState
	{
		[SerializeField]
		private UIShowAndHide introFadeInOut;

		public override bool AwakeOnSystem()
		{
			if(introFadeInOut == null) return SystemIsReady;
			introFadeInOut.InitHide();
			return false;
		}
		public override async Awaitable StartWaitSystem()
		{
			if(introFadeInOut == null) return;
			await introFadeInOut.OnShow();
		}
		public override async Awaitable EndedWaitSystem()
		{
			if(introFadeInOut == null) return;
			await introFadeInOut.OnHide();
		}
		public override void DestroyOnSystems()
		{
			if(introFadeInOut == null) return;
			introFadeInOut.InitHide();
		}
	}
}
