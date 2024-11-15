using UnityEngine;

namespace TF.Content
{
	public abstract class UIShowAndHide : UIUtilsComponent
	{
		public virtual void InitShow() { }
		public virtual void InitHide() { }
		public virtual async Awaitable OnShow() { }
		public virtual async Awaitable OnHide() { }
	}
}
