using System;

using BC.ODCC;

using UnityEngine;

using Object = UnityEngine.Object;

namespace TFSystem
{
	public interface IResourcesController : IOdccComponent
	{
		public enum AssetLoadAPI
		{
			AddressableAPI,
			ResourcesAPI,
		}

		public struct ResourcesKey
		{
			private string path;
			private AssetLoadAPI loadAPI;
			private bool isLoaded;
			public ResourcesKey(string path, AssetLoadAPI loadAPI)
			{
				this.path=path;
				this.loadAPI=loadAPI;
				this.isLoaded = false;
			}
			internal ResourcesKey(string path, AssetLoadAPI loadAPI, bool isLoaded)
			{
				this.path=path;
				this.loadAPI=loadAPI;
				this.isLoaded = isLoaded;
			}
			public string Path => path;
			public AssetLoadAPI LoadAPI => loadAPI;
			public bool IsLoaded => isLoaded;
		}

		public void Load<T>(string path, AssetLoadAPI loadAPI, Action<ResourcesKey> onLoaded) where T : Object;
		public void GetAsset<T>(ResourcesKey resourcesKey, Action<T> onComplete) where T : Object;
		public void Instantiate(ResourcesKey resourcesKey, Transform parent, Action<GameObject> onComplete);
		public void Instantiate(ResourcesKey resourcesKey, Vector3 position, Quaternion rotation, Transform parent, Action<GameObject> onComplete);
		public Awaitable<ResourcesKey> Load<T>(string path, AssetLoadAPI loadAPI) where T : Object;
		public Awaitable<T> GetAsset<T>(ResourcesKey resourcesKey) where T : Object;
		public Awaitable<GameObject> Instantiate(ResourcesKey resourcesKey, Transform parent);
		public Awaitable<GameObject> Instantiate(ResourcesKey resourcesKey, Vector3 position, Quaternion rotation, Transform parent);
		void DestroyObject(ResourcesKey loadKey, GameObject asset);
	}
}
