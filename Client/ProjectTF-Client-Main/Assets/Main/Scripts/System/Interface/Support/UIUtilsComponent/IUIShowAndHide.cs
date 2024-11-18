using BC.ODCC;

using UnityEngine;

namespace TF.System
{
	public interface IUIShowAndHide : IOdccComponent
	{
		public void InitShow();
		public void InitHide();
		public Awaitable OnShow();
		public Awaitable OnHide();
	}
}
