using System;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;

namespace TF.System.UI
{
	[Serializable, InlineProperty, HideLabel]
	public class TextInputField : UIViewItem<string>
	{
		public TMP_InputField titleInput;
		[LabelText("설명문"), LabelWidth(50), Multiline(2)]
		public string placeholderText;
		public int characterLimit = 25;

		public bool interaction = true;
		public Action<string> onSubmit;
		public Action<string> onValueChanged;
		protected override void InitView()
		{
			titleInput.characterLimit = 25;
			titleInput.lineLimit = 0;
			var placeholder = titleInput.placeholder;
			if(placeholder != null && placeholder.TryGetComponent<TMP_Text>(out var tmp_Text))
			{
				tmp_Text.text = placeholderText;
			}

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

		public override void ResetValue()
		{
			SetValue("");
		}
		public override string GetValue()
		{
			return titleInput.text;
		}
		public override void SetValue(string setValue, bool _interaction = true)
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
