using BC.ODCC;

using UnityEngine;

namespace TFSystem
{
	public interface IUIShowAndHide : IOdccComponent
	{
		public void InitShow();
		public void InitHide();
		public Awaitable OnShow();
		public Awaitable OnHide();
	}
}
