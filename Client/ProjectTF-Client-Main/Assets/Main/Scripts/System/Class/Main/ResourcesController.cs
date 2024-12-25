using System;
using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using static TFSystem.IResourcesController;

using Object = UnityEngine.Object;

namespace TFSystem
{
	internal class ResourcesController : ComponentBehaviour, IResourcesController
	{
		[Serializable]
		private record LoadStruct : IDisposable
		{
			public ResourcesKey loadKey;
			public Object loadAsset;

			public LoadStruct(ResourcesKey loadKey, Object loadAsset)
			{
				this.loadKey=loadKey;
				this.loadAsset=loadAsset;
			}

			public void Dispose()
			{
				loadKey = default;
				loadAsset = null;
			}
		}

		[Serializable]
		private record InstantiateStruct : IDisposable
		{
			public ResourcesKey loadKey;

			public List<GameObject> instantiateList;

			public InstantiateStruct(ResourcesKey loadKey)
			{
				this.loadKey=loadKey;
				instantiateList = new List<GameObject>();
			}
			public void Dispose()
			{
				loadKey = default;
				instantiateList = null;
			}
		}

		[SerializeField]
		private List<LoadStruct> loadStructList;
		[SerializeField]
		private List<InstantiateStruct> instantiateStructList;

		public struct AddressableAPI
		{
			public static async Awaitable<(ResourcesKey, T)> AsyncLoad<T>(string path) where T : Object
			{
				var handle = Addressables.LoadAssetAsync<T>(path);
				await handle.Task;
				return handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null
					? (new ResourcesKey(path, AssetLoadAPI.AddressableAPI), handle.Result)
					: (new ResourcesKey(path, AssetLoadAPI.AddressableAPI), null);
			}
			public static async Awaitable<GameObject> InstantiateAsync(ResourcesKey resourcesKey, Vector3 position, Quaternion rotation, Transform parent)
			{
				GameObject obj = await Addressables.InstantiateAsync(resourcesKey.Path, position, rotation, parent).Task;
				return obj;
			}
			public static void ReleaseInstance(GameObject gameObject)
			{
				Addressables.ReleaseInstance(gameObject);
			}
		}
		public struct ResourcesAPI
		{
			public static async Awaitable<(ResourcesKey, T)> AsyncLoad<T>(string path) where T : Object
			{
				var handle = Resources.LoadAsync(path);
				await handle;
				return handle.asset != null && handle.asset is T tAsset
					? (new ResourcesKey(path, AssetLoadAPI.ResourcesAPI), tAsset)
					: (new ResourcesKey(path, AssetLoadAPI.ResourcesAPI), null);
			}
			public static async Awaitable<GameObject> InstantiateAsync(GameObject loadObject, Vector3 position, Quaternion rotation, Transform parent)
			{
				var wait = await Object.InstantiateAsync(loadObject, parent, position, rotation);
				return wait[0];
			}
			public static void ReleaseInstance(GameObject _)
			{
				// Do Nothings;
			}
		}

		protected override void BaseAwake()
		{
			loadStructList = new List<LoadStruct>();
			instantiateStructList = new List<InstantiateStruct>();
		}
		public bool IsLoaded(ResourcesKey resourcesKey)
		{
			var find = loadStructList.Find(find=>find.loadKey.Path == resourcesKey.Path);
			return find is not null && find.loadAsset is not null;
		}
		public void Load<T>(string path, AssetLoadAPI loadAPI, Action<ResourcesKey> onLoaded) where T : Object
		{
			LoadAsync();
			async void LoadAsync()
			{
				ResourcesKey load = await Load<T>(path, loadAPI);
				onLoaded?.Invoke(load);
			}
		}
		public void GetAsset<T>(ResourcesKey resourcesKey, Action<T> onComplete) where T : Object
		{
			GetAssetAsync();
			async void GetAssetAsync()
			{
				T load = await GetAsset<T>(resourcesKey);
				onComplete?.Invoke(load);
			}
		}
		public void Instantiate(ResourcesKey resourcesKey, Vector3 position, Quaternion rotation, Transform parent, Action<GameObject> onComplete)
		{
			InstantiateAsync();
			async void InstantiateAsync()
			{
				var instantiate = await Instantiate(resourcesKey, position, rotation, parent);
				onComplete?.Invoke(instantiate);
			}
		}

		public async Awaitable<ResourcesKey> Load<T>(string path, AssetLoadAPI loadAPI) where T : Object
		{
			if(string.IsNullOrWhiteSpace(path)) return new ResourcesKey(path, loadAPI);

			int already = loadStructList.FindIndex(x => x.loadKey.Path == path);
			if(already >= 0)
			{
				if(loadStructList[already].loadAsset != null)
				{
					Debug.Log($"Already Asset: {path}");
				}
				else
				{
					do
					{
						Debug.LogWarning($"Loading Asset: {path}");
						await Awaitable.NextFrameAsync();
					}
					while(loadStructList[already].loadAsset == null);
				}
				return loadStructList[already].loadKey;
			}

			var loadStruct = new LoadStruct(new ResourcesKey(path, loadAPI), null);
			loadStructList.Add(loadStruct);

			(ResourcesKey key, T asset) asyncLoad = loadAPI switch {
				AssetLoadAPI.AddressableAPI => await AddressableAPI.AsyncLoad<T>(path),
				AssetLoadAPI.ResourcesAPI => await ResourcesAPI.AsyncLoad<T>(path),
				_ => (new ResourcesKey(path, loadAPI), null)
			};
			if(asyncLoad.asset != null)
			{
				loadStruct.loadKey = asyncLoad.key;
				loadStruct.loadAsset = asyncLoad.asset;
			}
			return asyncLoad.key;
		}
		public async Awaitable<T> GetAsset<T>(ResourcesKey resourcesKey) where T : Object
		{
			ResourcesKey load = await Load<T>(resourcesKey.Path, resourcesKey.LoadAPI);
			var find = loadStructList.Find(find=>find.loadKey.Path == resourcesKey.Path);
			return (find is not null && find.loadAsset is not null and T tFind) ? tFind : null;
		}

		public async Awaitable<GameObject> Instantiate(ResourcesKey resourcesKey, Vector3 position, Quaternion rotation, Transform parent)
		{
			GameObject loadObject = await GetAsset<GameObject>(resourcesKey);
			GameObject instantiateObject = await WaitInstantiate(loadObject);

			return instantiateObject;

			async Awaitable<GameObject> WaitInstantiate(GameObject loadObject)
			{
				if(loadObject == null)
				{
					Debug.LogError($"Failed Is Not : GameObject");
					return null;
				}

				GameObject newInstantiate = resourcesKey.LoadAPI switch{
					AssetLoadAPI.AddressableAPI => await AddressableAPI.InstantiateAsync(resourcesKey, position, rotation, parent),
					AssetLoadAPI.ResourcesAPI => await ResourcesAPI.InstantiateAsync(loadObject, position, rotation, parent),
					_ => null,
				};
				return Complete(resourcesKey, newInstantiate);
			}

			GameObject Complete(ResourcesKey loadKey, GameObject newGameObject)
			{
				if(newGameObject == null)
				{
					return null;
				}
				int index = instantiateStructList.FindIndex(find => find.loadKey.Equals(loadKey));
				if(index < 0)
				{
					index = instantiateStructList.Count;
					instantiateStructList.Add(new InstantiateStruct(loadKey));
				}
				InstantiateStruct findStruct = instantiateStructList[index];

				ResourcesState resourcesState = newGameObject.GetComponent<ResourcesState>() ?? newGameObject.AddComponent<ResourcesState>();
				resourcesState.Init(this, loadKey);
				findStruct.instantiateList.Add(newGameObject);

				return newGameObject;
			}
		}

		public void DestroyObject(ResourcesKey loadKey, GameObject asset)
		{
			switch(loadKey.LoadAPI)
			{
				case AssetLoadAPI.AddressableAPI:
					AddressableAPI.ReleaseInstance(asset);
					break;
				case AssetLoadAPI.ResourcesAPI:
					ResourcesAPI.ReleaseInstance(asset);
					break;
			}

			int index = instantiateStructList.FindIndex(find => find.loadKey.Equals(loadKey));
			if(index < 0) return;
			var list = instantiateStructList[index].instantiateList;
			if(list.Remove(asset))
			{
				if(list.Count == 0)
				{
					instantiateStructList[index].Dispose();
					instantiateStructList.RemoveAt(index);

					int loadIndex = loadStructList.FindIndex(find => find.loadKey.Equals(loadKey));
					if(loadIndex >= 0)
					{
						var loadAPI = loadStructList[loadIndex].loadKey.LoadAPI;
						loadStructList[loadIndex].Dispose();
						loadStructList.RemoveAt(loadIndex);
					}
				}
			}
		}
	}
}
