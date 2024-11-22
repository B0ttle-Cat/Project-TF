using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using BC.Base;
using BC.ODCC;

namespace TF.Content.Character
{
	internal class CharacterBuilder : ComponentBehaviour
	{
		private readonly string[] prefabPaths = new string[]
		{
			"Prefabs/Character/Normal_Character",
		};

		private Transform parent = null;

		private GameObject[] prefabs;
		private Coroutine[] loadAsyncs;
		private UnityAction<GameObject>[] callbacks;

		private void Log(string msg)
		{
			UnityEngine.Debug.Log($"[CharacterBuilder] {msg}");
		}

		private IEnumerator LoadAsync<T>(UnityAction<T> callback, string path) where T : UnityEngine.Object
		{
			Log($"LoadAsync Start :: {path}");
			var request = Resources.LoadAsync<T>(path);
			yield return new WaitUntil(() => !request.isDone);
			if (request.asset != null)
			{
				Log($"LoadAsync Success");
				callback?.Invoke(request.asset as T);
			}
			else
			{
				Log($"LoadAsync Failed");
				callback?.Invoke(null);
			}
		}

		public bool GetObject(UnityAction<GameObject> callback, eCharacterType type = eCharacterType.Normal)
		{
			Log($"GetObject_{type} Start");
			bool isLoad = false;
			int key = (int)type;
			if (prefabs[key] != null)
			{
				Log($"GetObject_{type} :: This object has already been loaded.");
				isLoad = true;
				if (CreateObject(out var character, prefabs[key], parent))
				{
					Log($"GetObject_{type} :: CreateObject Success");
					callback?.Invoke(character);
				}
				else
				{
					Log($"GetObject_{type} :: CreateObject Failed");
					callback?.Invoke(null);
				}
			}
			if (!isLoad)
			{
				isLoad = true;
				if (loadAsyncs[key] != null)
				{
					Log($"GetObject_{type} :: LoadAsync is in progress and a completion callback is registered.");
					callbacks[key] += callback;
				}
				else
				{
					Log($"GetObject_{type} :: Start LoadAsync");
					callbacks[key] = callback;
					loadAsyncs[key] = StartCoroutine(LoadAsync<GameObject>(
						(prefab) =>
						{
							LoadAsync_Success(key, prefab, parent);
						}, prefabPaths[key]));
				}
			}
			return isLoad;

			// ================================================================

			bool CreateObject(out GameObject character, GameObject prefab, Transform parent)
			{
				character = null;
				if (prefab == null)
				{
					return false;
				}
				character = GameObject.Instantiate(prefab);
				character.transform.SetParent(parent);
				character.transform.localPosition = Vector3.zero;
				character.transform.localEulerAngles = Vector3.zero;
				character.transform.localScale = Vector3.one;
				return true;
			}

			void LoadAsync_Success(int key, GameObject prefab, Transform parent)
			{
				if (CreateObject(out var character, prefab, parent))
				{
					Log($"GetObject_{type} :: CreateObject Success");
					prefabs[key] = prefab;
					callbacks[key]?.Invoke(character);
				}
				else
				{
					Log($"GetObject_{type} :: CreateObject Failed");
					callbacks[key]?.Invoke(null);
				}
				callbacks[key] = null;
				loadAsyncs[key] = null;
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
