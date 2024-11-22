using BC.ODCC;

using TF.System.UI;

using UnityEngine;

namespace TF.Content
{
	public class UILoadingRotation : UISupportComponent, IOdccUpdate, UIViewItem, UIBinding<float>
	{
		public RectTransform rectTransform;

		private float updateRotationValue;
		private float currentRotationValue;
		public float rotateSpeed;
		public float updateStep;

		string UIViewItem.ViewItemName { get; set; }

		void UIViewItem.SetupView()
		{
			if(rectTransform == null)
			{
				rectTransform = GetComponent<RectTransform>();
			}
		}
		void UIViewItem.SetupValue()
		{
			if(rectTransform == null) return;

			updateRotationValue = 0;
			RotationUpdate(0f);
		}
		float UIBinding<float>.GetValue()
		{
			return updateRotationValue;
		}

		void UIBinding<float>.SetValue(float setValue, bool _interaction)
		{
			updateRotationValue = setValue;
			if(_interaction)
			{
				RotationUpdate(Time.deltaTime);
			}
		}
		void IOdccUpdate.BaseUpdate()
		{
			RotationUpdate(Time.deltaTime);
		}

		private void RotationUpdate(float deltaTime)
		{
			if(rectTransform == null) return;

			updateRotationValue += rotateSpeed * deltaTime;
			if(updateStep <= 0)
			{
				updateRotationValue %=360f;
				currentRotationValue = updateRotationValue;
				rectTransform.eulerAngles = new Vector3(0, 0, currentRotationValue);
			}
			else
			{
				float stepAngle = Mathf.Abs(currentRotationValue - updateRotationValue);
				if(updateStep<=stepAngle)
				{
					updateRotationValue %=360f;
					currentRotationValue = updateRotationValue;
					rectTransform.eulerAngles = new Vector3(0, 0, currentRotationValue);
				}
			}
		}

	}
}
