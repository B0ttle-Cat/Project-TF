using System;

using UnityEngine;

using BC.Base;
using BC.ODCC;
using TFSystem;

namespace TFContent.Character
{
    public interface ICharacterCreate
    {
        bool Create(int idx, eCharacterType type, Action<Character> success, Action failed = null);
	}

	internal class CharacterCreate : ComponentBehaviour, ICharacterCreate
	{
		private readonly string[] prefabPaths = new string[]
		{
			"Prefabs/Character/Normal_Character",
		};

		private IResourcesController resources;
		private Transform parent = null;

		private void Log(string msg)
		{
			UnityEngine.Debug.Log($"[CharacterCreate] {msg}");
		}

        public bool Create(int idx, eCharacterType type, Action<Character> success, Action failed = null)
        {
            Log($"Create_{idx}_{type} Start");
			bool isCreate = false;
			int key = (int)type;
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
				if (resources != null)
				{
					Log($"Create_{idx}_{type} GetObject");
					isCreate = true;
					resources.Load<GameObject>(prefabPaths[key], IResourcesController.AssetLoadAPI.ResourcesAPI,
						(key) =>
						{
							Load_Complete(key, GetData(idx, type), success, failed);
						});
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

			void Load_Complete(IResourcesController.ResourcesKey key, CharacterData data, Action<Character> success, Action failed)
			{
				resources.Instantiate(key, parent, (obj) => ObjectCreate_Complete(obj, data, success, failed));
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

		protected override void BaseStart()
		{
			base.BaseStart();
			if (ThisContainer.TryGetObject<CharacterSystem>(out var service))
			{
				resources = service.AppController.ResourcesController;
			}
		}

		protected override void BaseAwake()
		{
			base.BaseAwake();

			parent = new GameObject().transform;
			parent.gameObject.name = "Character_Parent";
			parent.SetParent(ThisObject.transform);
			parent.localPosition = Vector3.zero;
			parent.localRotation = Quaternion.identity;
			parent.localScale = Vector3.one;
		}
	}
}
