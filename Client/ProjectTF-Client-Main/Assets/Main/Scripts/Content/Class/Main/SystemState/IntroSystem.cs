using TFSystem;
using TFSystem.UI;

using UnityEngine;

namespace TFContent
{
	public class IntroSystem : SystemState
	{
		[SerializeField]
		private UIShowAndHide introFadeInOut;

		protected override void AwakeOnSystem()
		{
			if(introFadeInOut == null) return;
			introFadeInOut.InitHide();
		}
		protected override async Awaitable StartWaitSystem()
		{
			if(introFadeInOut == null) return;
			await introFadeInOut.OnShow();
			await Awaitable.WaitForSecondsAsync(0.5f);
		}
		protected override async Awaitable EndedWaitSystem()
		{
			if(introFadeInOut == null) return;
			await introFadeInOut.OnHide();
		}
		protected override void DestroyOnSystems()
		{
			if(introFadeInOut == null) return;
			introFadeInOut.InitHide();
		}
	}
}
