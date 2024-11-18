using Sirenix.OdinInspector;

namespace TF.System.UI
{
	public abstract class UIViewItem
	{
		[DisplayAsString]
		public string viewItemName;
		internal void Init() { InitView(); ResetValue(); }
		protected abstract void InitView();
		public abstract void ResetValue();
	}

	public abstract class UIViewItem<T> : UIViewItem
	{
		public abstract T GetValue();
		public abstract void SetValue(T setValue, bool _interaction = true);
	}
}
