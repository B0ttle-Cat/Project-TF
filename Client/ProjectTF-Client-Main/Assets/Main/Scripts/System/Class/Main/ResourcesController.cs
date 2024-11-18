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
				int already = loadStructList.FindIndex(x => x.path == path);
				if(already >= 0)
				{
					if(loadStructList[already].loadAsset != null)
					{
						Debug.LogWarning($"Already Asset: {path}");
						onComplete?.Invoke(true);
					}
					else
					{
						do
						{
							Debug.LogWarning($"Loading Asset: {path}");
							await Awaitable.NextFrameAsync();
						}
						while(loadStructList[already].loadAsset == null);

						onComplete?.Invoke(true);
					}
					return;
				}

				if(loadAPI == IResourcesController.AssetLoadAPI.AddressableAPI)
				{
					Addressables.LoadAssetAsync<T>(path).Completed += (AsyncOperationHandle<T> handle) => {
						if(handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
						{
							Complete(path, loadAPI, handle.Result);
							onComplete?.Invoke(true);
						}
						else
						{
							Debug.LogError($"Failed Load Asset: {path}");
							onComplete?.Invoke(false);
						}
					};
				}
				else if(loadAPI == IResourcesController.AssetLoadAPI.ResourcesAPI)
				{
					var wait = Resources.LoadAsync(path);
					wait.completed += (AsyncOperation obj) => {
						if(wait.asset != null)
						{
							Complete(path, loadAPI, wait.asset);
						}
						else
						{
							Debug.LogError($"Failed Load Asset: {path}");
							onComplete?.Invoke(false);
						}
					};
				}

			}
			void Complete(string path, IResourcesController.AssetLoadAPI loadAPI, Object loadAsset)
			{
				loadStructList.Add(new LoadStruct(path, loadAsset));
			}
		}
		public void GetAsset<T>(string path, IResourcesController.AssetLoadAPI loadAPI, Action<T> onComplete) where T : Object
		{
			Load<T>(path, loadAPI, (result) => {
				if(result)
				{
					var find = loadStructList.Find(find=>find.path == path);
					if(find is not null and T tFind)
					{
						onComplete?.Invoke(tFind);
						return;
					}
				}
				onComplete?.Invoke(null);
			});
		}
		public void Instantiate(string path, IResourcesController.AssetLoadAPI loadAPI, Vector3 position, Quaternion rotation, Transform parent, Action<GameObject> onComplete)
		{
			GetAsset<GameObject>(path, loadAPI, (loadObject) => {
				InstantiateAsync(loadObject);
			});
			async void InstantiateAsync(GameObject loadObject)
			{
				if(loadObject == null)
				{
					Debug.LogError($"Failed Is Not : GameObject");
					onComplete?.Invoke(null);
					return;
				}

				if(loadAPI == IResourcesController.AssetLoadAPI.AddressableAPI)
				{
					GameObject obj = await Addressables.InstantiateAsync(path, position, rotation, parent).Task;
					Complete(path, obj);
				}
				else if(loadAPI == IResourcesController.AssetLoadAPI.ResourcesAPI)
				{
					var wait = await Object.InstantiateAsync(loadObject, 0, parent, position, rotation);
					Complete(path, wait[0]);
				}
			}

			void Complete(string path, GameObject newGameObject)
			{
				if(newGameObject == null)
				{
					onComplete?.Invoke(null);
					return;
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

				onComplete?.Invoke(newGameObject);
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
