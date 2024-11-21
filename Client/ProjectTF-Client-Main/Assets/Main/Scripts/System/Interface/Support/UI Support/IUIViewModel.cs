using BC.ODCC;

using TF.System.UI;

using UnityEngine;

namespace TF.System
{
	public interface IUIViewModel : IOdccComponent
	{
		public IUIShowAndHide ThisUIShowAndHide { get; }

		public void InitShow() => ThisUIShowAndHide.InitShow();
		public void InitHide() => ThisUIShowAndHide.InitHide();
		public async Awaitable OnShow() => await ThisUIShowAndHide.OnShow();
		public async Awaitable OnHide() => await ThisUIShowAndHide.OnHide();

		public bool TryGetBinding<T>(string nameOfViewItem, out UIBinding<T> viewItem);
		public bool TryGetEventHandle<THandle>(string nameOfViewItem, out THandle viewItem) where THandle : class, UIEventHandle;
	}
}
