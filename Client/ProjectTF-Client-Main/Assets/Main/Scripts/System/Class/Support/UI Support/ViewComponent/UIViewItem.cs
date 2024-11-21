using System;

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

	public interface UIBinding<T>
	{
		public abstract T GetValue();
		public abstract void SetValue(T setValue, bool _interaction = true);
	}
	public interface UIEventHandle
	{
		public bool interaction { get; set; }
	}
	public interface UIEvent_OnClick : UIEventHandle
	{
		public Action onClick { get; set; }
	}
	public interface UIEvent_OnSelect : UIEventHandle
	{
		public Action onSelect { get; set; }
	}
	public interface UIEvent_OnSubmit : UIEventHandle
	{
		public Action onSubmit { get; set; }
	}
	public interface UIEvent_OnClick<T> : UIEventHandle
	{
		public Action<T> onClick { get; set; }
	}
	public interface UIEvent_OnSelect<T> : UIEventHandle
	{
		public Action<T> onSelect { get; set; }
	}
	public interface UIEvent_OnSubmit<T> : UIEventHandle
	{
		public Action<T> onSubmit { get; set; }
	}
	public interface UIEvent_OnChangeValue<T> : UIEventHandle
	{
		public Action<T> onValueChanged { get; set; }
	}

}
