using BC.ODCC;

using UnityEngine;

namespace TF.System
{
	public interface IUIShowAndHideControl : IOdccComponent
	{
		public UIShowAndHide ThisUIShowAndHide { get; set; }
		public void InitShow() => ThisUIShowAndHide.InitShow();
		public void InitHide() => ThisUIShowAndHide.InitHide();
		public async Awaitable OnShow() => await ThisUIShowAndHide.OnShow();
		public async Awaitable OnHide() => await ThisUIShowAndHide.OnHide();
	}
}
