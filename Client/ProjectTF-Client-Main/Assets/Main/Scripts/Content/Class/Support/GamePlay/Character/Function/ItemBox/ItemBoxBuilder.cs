using UnityEngine;

using BC.ODCC;

namespace TFContent.Character
{
    internal class ItemBoxBuilder : ComponentBehaviour
	{
		[SerializeField] private GameObject boxPrefab;
		[SerializeField] private Transform boxParent;

		private bool GetBox(out BoxObject output)
		{
			output = null;
			if (boxPrefab == null)
			{
				return false;
			}
			GameObject box = GameObject.Instantiate(boxPrefab, boxParent);
			box.SetActive(true);
			return box.TryGetComponent<BoxObject>(out output);
		}

		public void @Reset()
		{
			if (ThisContainer.TryGetComponent<ItemBoxSearch>(out var search))
			{
				if (search.SearchAll(out var boxes))
				{
					foreach (var box in boxes)
					{
						if (box != null && box.gameObject.activeSelf)
						{
							GameObject.Destroy(box.GameObject);
						}
					}
				}
			}
			if (ThisContainer.TryGetData<CharacterItemBoxData>(out var data))
			{
				data.currentSize = Vector2Int.zero;
			}
		}

		public void Add(Vector2Int size)
		{
			if (size == Vector2Int.zero)
			{
				return;
			}
			if (ThisContainer.TryGetData<CharacterItemBoxData>(out var data))
			{
				Vector2Int start = data.currentSize + Vector2Int.one;
				Vector2Int target = start + size;
				for (int y = start.y; y < target.y; y++)
				{
					for (int x = 0; x < size.x; x++)
					{
						if (GetBox(out var box))
						{
							Vector2Int point = new Vector2Int(x, y);
							box.gameObject.name = $"Box_{point}";
							box.point = point;
						}
					}
				}
				data.currentSize += size;
			}
		}

		public void Create(Vector2Int start, Vector2Int size)
		{
			if (size == Vector2Int.zero)
			{
				return;
			}
			for (int y = start.y; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					if (GetBox(out var box))
					{
						Vector2Int point = new Vector2Int(x, y);
						box.gameObject.name = $"Box_{point}";
						box.point = point;
					}
				}
			}
			if (ThisContainer.TryGetData<CharacterItemBoxData>(out var data))
			{
				data.currentSize = size;
			}
		}
	}
}