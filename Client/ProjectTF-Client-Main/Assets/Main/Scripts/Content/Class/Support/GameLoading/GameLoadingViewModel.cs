using System.Collections.Generic;

using Sirenix.OdinInspector;

using TF.System.UI;

using UnityEngine;

namespace TF.Content
{
	public class GameLoadingViewModel : UIViewModelComponent
	{


		[SerializeField, Multiline(2), ListDrawerSettings(ShowFoldout = false, ShowPaging = false ,DraggableItems = false)]
		private List<string> helpTextList;

		protected override void AwakeUIView(ref ViewItemSetter viewItemBuilder)
		{
			if(ThisContainer.TryGetComponent<UILoadingRotation>(out var rotationViewItem)
				&& ThisContainer.TryGetComponent<UILoadingHelpText>(out var helpTextViewItem))
			{
				viewItemBuilder.Add(rotationViewItem, nameof(rotationViewItem));
				viewItemBuilder.Add(helpTextViewItem, nameof(helpTextViewItem));
			}
		}

		protected override void BaseStart()
		{


		}
	}
}
