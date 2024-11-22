using System;

using BC.ODCC;

using UnityEngine;

using Object = UnityEngine.Object;

namespace TF.System
{
	public interface IResourcesController : IOdccComponent
	{
		public enum AssetLoadAPI
		{
			AddressableAPI,
			ResourcesAPI,
		}

		public void Load<T>(string path, AssetLoadAPI loadAPI, Action<bool> onComplete) where T : Object;
		public void GetAsset<T>(string path, AssetLoadAPI loadAPI, Action<T> onComplete) where T : Object;
		public void Instantiate(string path, AssetLoadAPI loadAPI, Vector3 position, Quaternion rotation, Transform parent, Action<GameObject> onComplete);
		public Awaitable<bool> Load<T>(string path, AssetLoadAPI loadAPI) where T : Object;
		public Awaitable<T> GetAsset<T>(string path, AssetLoadAPI loadAPI) where T : Object;
		public Awaitable<GameObject> Instantiate(string path, AssetLoadAPI loadAPI, Vector3 position, Quaternion rotation, Transform parent);
		void DestroyObject(string path, GameObject asset);
	}
}
