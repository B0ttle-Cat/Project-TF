using UnityEngine;

namespace TF.System.UI
{
	public abstract class UIShowAndHide : UIViewComponent, IUIShowAndHide
	{
		public UIShowAndHide ThisUIShowAndHide { get; }
		public virtual void InitShow() { }
		public virtual void InitHide() { }
		public virtual async Awaitable OnShow() { }
		public virtual async Awaitable OnHide() { }
	}
}
