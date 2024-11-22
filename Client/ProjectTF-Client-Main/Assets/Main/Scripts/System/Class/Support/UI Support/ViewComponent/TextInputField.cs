using System;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;

namespace TF.System.UI
{
	[Serializable, InlineProperty, HideLabel]
	public class TextInputField : UIViewItem, UIBinding<string>, UIEvent_OnSelect<string>, UIEvent_OnSubmit<string>, UIEvent_OnChangeValue<string>
	{
		public string ViewItemName { get; set; }

		public TMP_InputField titleInput;
		[LabelText("설명문"), LabelWidth(50), Multiline(2)]
		public string placeholderText;
		public int characterLimit = 25;

		public bool interaction { get; set; }
		public Action<string> onSelect { get; set; }
		public Action<string> onSubmit { get; set; }
		public Action<string> onValueChanged { get; set; }
		public void Init() { SetupView(); SetupValue(); }
		public virtual void SetupView()
		{
			interaction = true;
			titleInput.characterLimit = 25;
			titleInput.lineLimit = 0;
			var placeholder = titleInput.placeholder;
			if(placeholder != null && placeholder.TryGetComponent<TMP_Text>(out var tmp_Text))
			{
				tmp_Text.text = placeholderText;
			}

			titleInput.onSelect.RemoveAllListeners();
			titleInput.onSelect.AddListener((value) => {
				if(!interaction) return;
				onSelect?.Invoke(value);
			});
			titleInput.onSubmit.RemoveAllListeners();
			titleInput.onSubmit.AddListener((value) => {
				if(!interaction) return;

				onSubmit?.Invoke(value);
			});

			titleInput.onValueChanged.RemoveAllListeners();
			titleInput.onValueChanged.AddListener((value) => {
				if(!interaction) return;

				onValueChanged?.Invoke(value);
			});
		}

		public virtual void SetupValue()
		{
			SetValue("");
		}
		public virtual string GetValue()
		{
			return titleInput.text;
		}
		public virtual void SetValue(string setValue, bool _interaction = true)
		{
			if(_interaction)
			{
				Set();
			}
			else
			{
				var old = interaction;
				interaction = false;
				Set();
				interaction = old;
			}
			void Set()
			{
				titleInput.text = setValue;
			}
		}
	}
}
