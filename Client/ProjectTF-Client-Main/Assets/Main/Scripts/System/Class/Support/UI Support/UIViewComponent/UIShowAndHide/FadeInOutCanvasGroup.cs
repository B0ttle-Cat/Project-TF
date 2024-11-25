using UnityEngine;

namespace TFSystem.UI
{
	public class FadeInOutCanvasGroup : UIShowAndHide
	{
		private CanvasGroup _canvasGroup;
		public CanvasGroup ThisCanvasGroup {
			get {
				return _canvasGroup = _canvasGroup != null ? _canvasGroup : GetComponentInChildren<CanvasGroup>(true) ?? gameObject.AddComponent<CanvasGroup>();
			}
		}

		[SerializeField]
		private float fadeInSpeed = 3f;
		[SerializeField]
		private float fadeOutSpeed = 3f;

		private bool isUpdateShow;
		private bool isUpdateHide;

		public override void InitShow()
		{
			var canvasGroup = ThisCanvasGroup;

			canvasGroup.alpha = 1;
			canvasGroup.blocksRaycasts = true;

			isUpdateShow = false;
			isUpdateHide = false;
		}
		public override void InitHide()
		{
			var canvasGroup = ThisCanvasGroup;

			canvasGroup.alpha = 0;
			canvasGroup.blocksRaycasts = false;

			isUpdateShow = false;
			isUpdateHide = false;
		}
		public override async Awaitable OnShow()
		{
			isUpdateShow = true;
			isUpdateHide = false;

			var canvasGroup = ThisCanvasGroup;
			canvasGroup.blocksRaycasts = true;
			float alpha = canvasGroup.alpha;
			while(alpha < 1f && isUpdateShow)
			{
				await Awaitable.NextFrameAsync();
				if(canvasGroup == null) return;
				alpha += Time.deltaTime * fadeInSpeed;
				canvasGroup.alpha = alpha;
			}
			canvasGroup.alpha = alpha = 1f;

			await Awaitable.NextFrameAsync();
			isUpdateShow = false;
		}
		public override async Awaitable OnHide()
		{
			isUpdateShow = false;
			isUpdateHide = true;

			var canvasGroup = ThisCanvasGroup;
			float alpha = canvasGroup.alpha;
			while(alpha >= 0f && isUpdateHide)
			{
				await Awaitable.NextFrameAsync();
				if(canvasGroup == null) return;
				canvasGroup.alpha = alpha;
				alpha -= Time.deltaTime * fadeOutSpeed;
			}
			canvasGroup.alpha = alpha = 0f;
			canvasGroup.blocksRaycasts = false;

			await Awaitable.NextFrameAsync();
			isUpdateHide = false;
		}
	}
}
