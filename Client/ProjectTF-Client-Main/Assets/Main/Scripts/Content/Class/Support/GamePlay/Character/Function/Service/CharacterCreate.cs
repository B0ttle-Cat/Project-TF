using System;

using UnityEngine;

using BC.Base;
using BC.ODCC;
using TFSystem;

namespace TFContent.Character
{
	public interface ICharacterCreate : IOdccComponent
	{
		bool Create<T>(int idx, eCharacterType type, Action<T> success, Action failed = null) where T : Character;
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

		public bool Create<T>(int idx, eCharacterType type, Action<T> success, Action failed = null) where T : Character
		{
			Log($"Create_{idx}_{type} Start");
			bool isCreate = false;
			int key = (int)type;
			if (ThisContainer.TryGetComponent<CharacterSearch>(out var search))
			{
				if (search.Search_Character(out var character, idx))
				{
					Log($"Create_{idx}_{type} Search Success");
					isCreate = true;
					success?.Invoke(character as T);
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

			void Load_Complete(IResourcesController.ResourcesKey key, CharacterData data, Action<T> success, Action failed)
			{
				resources.Instantiate(key, parent, (obj) => Create_Complete(obj, data, success, failed));
			}

			void Create_Complete(GameObject obj, CharacterData data, Action<T> success, Action failed)
			{
				if (obj == null)
				{
					Log($"Create_{data.idx}_{data.type} Failed :: Object Null");
					failed?.Invoke();
					return;
				}
				T character = null;
				if (!obj.TryGetComponent(out character))
				{
					character = obj.AddComponent<T>();
				}

				obj.SetActive(true);
				obj.name = GetObjectName(data.idx);
				character.SetCharacterData(data);

				if (character is MyCharacter _)
				{
					if (ThisContainer.TryGetComponent<CharacterSearch>(out var search))
					{
						if (search.Search_ItemBox(out var itemBox))
						{
							if (itemBox.ThisContainer.TryGetComponent<ItemBoxBuilder>(out var builder))
							{
								builder.Create(Vector2Int.zero, data.itemBoxSize);
							}
						}
					}
				}

				Log($"Create_{data.idx}_{data.type} Success");
				success?.Invoke(character as T);
			}

			string GetObjectName(int idx)
			{
				return $"Character_{idx}";
			}
		}

		protected override void BaseStart()
		{
			base.BaseStart();
			if (ThisContainer.TryGetObject<CharacterSystem>(out var system))
			{
				resources = system.AppController.ResourcesController;
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
