using BC.ODCC;

using UnityEngine;

namespace TF.Content
{
	public interface IUIShowAndHide : IOdccComponent
	{
		public UIShowAndHide ThisUIShowAndHide { get; set; }
		public void InitShow() => ThisUIShowAndHide.InitShow();
		public void InitHide() => ThisUIShowAndHide.InitHide();
		public async Awaitable OnShow() => await ThisUIShowAndHide.OnShow();
		public async Awaitable OnHide() => await ThisUIShowAndHide.OnHide();
	}


	public abstract class UIShowAndHide : UIUtilsComponent
	{
		public virtual void InitShow() { }
		public virtual void InitHide() { }
		public virtual async Awaitable OnShow() { }
		public virtual async Awaitable OnHide() { }
	}
}
