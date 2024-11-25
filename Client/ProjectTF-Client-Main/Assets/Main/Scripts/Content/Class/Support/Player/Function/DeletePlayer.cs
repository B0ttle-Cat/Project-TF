using UnityEngine;

using BC.ODCC;

namespace TFContent.Player
{
	internal class DeletePlayer : ComponentBehaviour
	{
		private void Log(string msg)
		{
			UnityEngine.Debug.Log($"[DeletePlayer] {msg}");
		}

		public bool Delete(int playerIdx)
		{
			Log($"Delete_{playerIdx} Start");
			if (ThisContainer.TryGetComponent<SearchPlayer>(out var search))
			{
				if (search.Search(out var player, playerIdx))
				{
					Log($"Delete_{playerIdx} Success");
					GameObject.Destroy(search.gameObject);
					return true;
				}
			}
			Log($"Delete_{playerIdx} Failed");
			return false;
		}
	}
}
