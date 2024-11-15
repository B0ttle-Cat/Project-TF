using UnityEngine;

namespace TF.Content
{
	public class FadeInOut : UIShowAndHide
	{
		[SerializeField]
		private CanvasGroup canvasGroup;

		[SerializeField]
		private float fadeInSpeed = 3f;
		[SerializeField]
		private float fadeOutSpeed = 3f;
		[SerializeField]
		private float showWaitTime = .5f;

		public override void BaseAwake()
		{
			base.BaseAwake();
		}
		public override void InitShow()
		{
			if(canvasGroup == null) return;
			canvasGroup.alpha = 1;
		}
		public override void InitHide()
		{
			if(canvasGroup == null) return;
			canvasGroup.alpha = 0;
		}
		public override async Awaitable OnShow()
		{
			if(canvasGroup == null) return;

			float alpha = canvasGroup.alpha;
			while(alpha < 1f)
			{
				await Awaitable.NextFrameAsync();
				if(canvasGroup == null) return;
				alpha += Time.deltaTime * fadeInSpeed;
				canvasGroup.alpha = alpha;
			}
			canvasGroup.alpha = alpha = 1f;
			await Awaitable.WaitForSecondsAsync(showWaitTime);
		}
		public override async Awaitable OnHide()
		{
			if(canvasGroup == null) return;

			float alpha = canvasGroup.alpha;
			while(alpha >= 0f)
			{
				await Awaitable.NextFrameAsync();
				if(canvasGroup == null) return;
				canvasGroup.alpha = alpha;
				alpha -= Time.deltaTime * fadeOutSpeed;
			}
			canvasGroup.alpha = alpha = 0f;
		}
	}
}
