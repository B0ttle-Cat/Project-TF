﻿using TF.System;
using TF.System.UI;

using UnityEngine;

namespace TF.Content
{
	public class IntroSystem : SystemState
	{
		[SerializeField]
		private UIShowAndHide introFadeInOut;

		public override void AwakeOnSystem()
		{
			if(introFadeInOut == null) return;
			introFadeInOut.InitHide();
		}
		public override async Awaitable StartWaitSystem()
		{
			if(introFadeInOut == null) return;
			await introFadeInOut.OnShow();
			await Awaitable.WaitForSecondsAsync(0.5f);
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