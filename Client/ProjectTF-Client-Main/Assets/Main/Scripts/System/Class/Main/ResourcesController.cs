using System;
using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Object = UnityEngine.Object;

namespace TF.System
{
	public class ResourcesController : ComponentBehaviour, IResourcesController
	{
		[Serializable]
		public record LoadStruct : IDisposable
		{
			public string path;
			public Object loadAsset;

			public LoadStruct(string path, Object loadAsset)
			{
				this.path=path;
				this.loadAsset=loadAsset;
			}

			public void Dispose()
			{
				path = null;
				loadAsset = null;
			}
		}

		[Serializable]
		public record InstantiateStruct : IDisposable
		{
			public string path;
			public List<GameObject> instantiateList;

			public InstantiateStruct(string path)
			{
				this.path=path;
				instantiateList = new List<GameObject>();
			}
			public void Dispose()
			{
				path = null;
				instantiateList = null;
			}
		}

		public List<LoadStruct> loadStructList;
		public List<InstantiateStruct> instantiateStructList;

		protected override void BaseAwake()
		{
			loadStructList = new List<LoadStruct>();
			instantiateStructList = new List<InstantiateStruct>();
		}


		public void Load<T>(string path, IResourcesController.AssetLoadAPI loadAPI, Action<bool> onComplete) where T : Object
		{
			LoadAsync();
			async void LoadAsync()
			{
				bool load = await Load<T>(path, loadAPI);
				onComplete?.Invoke(false);
			}
		}
		public void GetAsset<T>(string path, IResourcesController.AssetLoadAPI loadAPI, Action<T> onComplete) where T : Object
		{
			GetAssetAsync();
			async void GetAssetAsync()
			{
				T load = await GetAsset<T>(path, loadAPI);
				onComplete?.Invoke(load);
			}
		}
		public void Instantiate(string path, IResourcesController.AssetLoadAPI loadAPI, Vector3 position, Quaternion rotation, Transform parent, Action<GameObject> onComplete)
		{
			InstantiateAsync();
			async void InstantiateAsync()
			{
				var instantiate = await Instantiate(path, loadAPI, position, rotation, parent);
				onComplete?.Invoke(instantiate);
			}
		}


		public async Awaitable<bool> Load<T>(string path, IResourcesController.AssetLoadAPI loadAPI) where T : Object
		{
			if(string.IsNullOrWhiteSpace(path)) return false;

			int already = loadStructList.FindIndex(x => x.path == path);
			if(already >= 0)
			{
				if(loadStructList[already].loadAsset != null)
				{
					Debug.LogWarning($"Already Asset: {path}");
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
				return false;
			}

			if(loadAPI == IResourcesController.AssetLoadAPI.AddressableAPI)
			{
				var handle = Addressables.LoadAssetAsync<T>(path);
				await handle.Task;
				return Complete(path, loadAPI, handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null);
			}
			else if(loadAPI == IResourcesController.AssetLoadAPI.ResourcesAPI)
			{
				var handle = Resources.LoadAsync(path);
				await handle;
				return Complete(path, loadAPI, handle.asset);
			}

			return false;

			bool Complete(string path, IResourcesController.AssetLoadAPI loadAPI, Object loadAsset)
			{
				if(loadAsset == null)
				{
					Debug.LogError($"Failed Load Asset: {path}");
					return false;
				}
				loadStructList.Add(new LoadStruct(path, loadAsset));
				return true;
			}
		}
		public async Awaitable<T> GetAsset<T>(string path, IResourcesController.AssetLoadAPI loadAPI) where T : Object
		{
			bool load = await Load<T>(path, loadAPI);
			if(load)
			{
				var find = loadStructList.Find(find=>find.path == path);
				if(find is not null and T tFind)
				{
					return tFind;
				}
			}
			return null;
		}
		public async Awaitable<GameObject> Instantiate(string path, IResourcesController.AssetLoadAPI loadAPI, Vector3 position, Quaternion rotation, Transform parent)
		{
			GameObject loadObject = await GetAsset<GameObject>(path, loadAPI);
			GameObject instantiateObject = await WaitInstantiate(loadObject);

			return instantiateObject;

			async Awaitable<GameObject> WaitInstantiate(GameObject loadObject)
			{
				if(loadObject == null)
				{
					Debug.LogError($"Failed Is Not : GameObject");
					return null;
				}

				if(loadAPI == IResourcesController.AssetLoadAPI.AddressableAPI)
				{
					GameObject obj = await Addressables.InstantiateAsync(path, position, rotation, parent).Task;
					return Complete(path, obj);
				}
				else if(loadAPI == IResourcesController.AssetLoadAPI.ResourcesAPI)
				{
					var wait = await Object.InstantiateAsync(loadObject, 0, parent, position, rotation);
					return Complete(path, wait[0]);
				}
				return null;
			}

			GameObject Complete(string path, GameObject newGameObject)
			{
				if(newGameObject == null)
				{
					return null;
				}

				int index = instantiateStructList.FindIndex(find => find.path == path);
				if(index < 0)
				{
					index = instantiateStructList.Count;
					instantiateStructList.Add(new InstantiateStruct(path));
				}
				InstantiateStruct findStruct = instantiateStructList[index];

				ResourcesState resourcesState = newGameObject.GetComponent<ResourcesState>() ?? newGameObject.AddComponent<ResourcesState>();
				resourcesState.Init(this, path);
				findStruct.instantiateList.Add(newGameObject);

				return newGameObject;
			}
		}

		public void DestroyObject(string path, GameObject asset)
		{
			int index = instantiateStructList.FindIndex(find => find.path == path);
			if(index < 0) return;
			var list = instantiateStructList[index].instantiateList;
			if(list.Remove(asset))
			{
				if(list.Count == 0)
				{
					instantiateStructList[index].Dispose();
					instantiateStructList.RemoveAt(index);

					int loadIndex = loadStructList.FindIndex(find => find.path == path);
					if(loadIndex >= 0)
					{
						loadStructList[loadIndex].Dispose();
						loadStructList.RemoveAt(loadIndex);
					}
				}
			}
		}
	}
}
