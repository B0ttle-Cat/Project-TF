using BC.ODCC;

using UnityEngine;

namespace TF.System
{
	public interface IUIViewComponent : IOdccComponent
	{
		public IUIShowAndHide ThisUIShowAndHide { get; }

		public void InitShow() => ThisUIShowAndHide.InitShow();
		public void InitHide() => ThisUIShowAndHide.InitHide();
		public async Awaitable OnShow() => await ThisUIShowAndHide.OnShow();
		public async Awaitable OnHide() => await ThisUIShowAndHide.OnHide();
	}
}
