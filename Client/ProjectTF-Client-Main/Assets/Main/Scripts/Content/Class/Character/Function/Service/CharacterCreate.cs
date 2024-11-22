using System;

using UnityEngine;

using BC.Base;
using BC.ODCC;

namespace TF.Content.Character
{
    public interface ICharacterCreate
    {
        bool Create(int idx, eCharacterType type, Action<Character> success, Action failed = null);
	}

	internal class CharacterCreate : ComponentBehaviour, ICharacterCreate
	{
		private void Log(string msg)
		{
			UnityEngine.Debug.Log($"[CharacterCreate] {msg}");
		}

        public bool Create(int idx, eCharacterType type, Action<Character> success, Action failed = null)
        {
            Log($"Create_{idx}_{type} Start");
			bool isCreate = false;
            if (ThisContainer.TryGetComponent<CharacterSearch>(out var search))
            {
                if (search.Search(out var character, idx))
				{
					Log($"Create_{idx}_{type} Search Success");
					isCreate = true;
					success?.Invoke(character);
				}
            }
            if (!isCreate)
            {
                if (ThisContainer.TryGetComponent<CharacterBuilder>(out var builder))
				{
					Log($"Create_{idx}_{type} GetObject");
					isCreate = true;
					builder.GetObject((obj) => ObjectCreate_Complete(obj, GetData(idx, type), success, failed));
				}
                else
				{
					Log($"Create_{idx}_{type} Failed");
					failed?.Invoke();
				}
			}
            return isCreate;

			// ================================================================

			CharacterData GetData(int idx, eCharacterType type)
            {
                return new CharacterData()
                {
                    idx = idx,
                    type = type,
                };
            }

			void ObjectCreate_Complete(GameObject obj, CharacterData data, Action<Character> success, Action failed)
            {
                if (obj == null)
				{
					Log($"Create_{idx}_{type} Failed :: Object Null");
					failed?.Invoke();
					return;
                }
                Character character = null;
				if (!obj.TryGetComponent(out character))
                {
                    character = obj.AddComponent<Character>();
				}
                obj.name = GetCharacterName(idx);
                character.SetCharacterData(data);

				Log($"Create_{idx}_{type} Success");
				success?.Invoke(character);
			}

			string GetCharacterName(int idx)
			{
				return $"Character_{idx}";
			}
		}
    }
}
