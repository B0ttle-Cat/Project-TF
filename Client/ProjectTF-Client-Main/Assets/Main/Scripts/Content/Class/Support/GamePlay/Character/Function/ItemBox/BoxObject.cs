using UnityEngine;
using UnityEngine.EventSystems;

using BC.ODCC;

namespace TFContent.Character
{
    public class BoxObject : ObjectBehaviour, IDragHandler, IPointerClickHandler
	{
		public int itemId = -1;
		public Vector2Int point;

		public void Removeitem()
		{

		}

		public void SetItem()
		{

		}

		private void MoveItem()
		{
			if (itemId < 0)
			{
				return;
			}
		}

		#region Unity Events

		public void OnDrag(PointerEventData eventData)
		{
			Debug.Log($"OnDrag :: {this.gameObject.name}");
			MoveItem();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Debug.Log($"OnPointerClick :: {this.gameObject.name}");
			MoveItem();
		}

		#endregion

		protected override void BaseAwake()
		{
			base.BaseAwake();

		}
	}
}