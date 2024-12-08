using BC.ODCC;

using UnityEngine;

namespace TFContent.Character
{
	internal class CharacterDelete : ComponentBehaviour
	{
		private void Log(string msg)
		{
			UnityEngine.Debug.Log($"[CharacterDelete] {msg}");
		}

		public bool Delete(int idx)
		{
			Log($"Delete_{idx} Start");
			if(ThisContainer.TryGetComponent<CharacterSearch>(out var search))
			{
				if(search.Search_Character(out var character, idx))
				{
					Log($"Delete_{idx} Success");
					GameObject.Destroy(character.gameObject);
					return true;
				}
			}
			Log($"Delete_{idx} Failed");
			return false;
		}
	}
}
