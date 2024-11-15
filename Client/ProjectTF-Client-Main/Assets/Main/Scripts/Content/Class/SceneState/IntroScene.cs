using TF.System;

using UnityEngine;

namespace TF.Content
{
	public class IntroScene : SceneState
	{
		public override string TargetSceneName => nameof(IntroScene);

		protected override async Awaitable DelayOpenScene()
		{
			int length = SystemStateInTargetScene.Length;
			for(int i = 0 ; i < length ; i++)
			{
				SystemState systemState = SystemStateInTargetScene[i];
			}
		}
		protected override async Awaitable DelayCloseScene()
		{

		}
	}
}
