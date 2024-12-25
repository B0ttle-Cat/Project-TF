using System;
using System.Collections.Generic;

using BC.ODCC;

using Sirenix.OdinInspector;

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

		[Serializable]
		public struct ResourcesKey
		{
#if UNITY_EDITOR
			public List<string> EditorAssetPathList()
			{
				var assetPathList = loadAPI == AssetLoadAPI.AddressableAPI
					? TFEditor.EditorUtility.FindAddressableAssetsInGroupName("",asset => true)
					: TFEditor.EditorUtility.FindResourcesrAssetsInFolderName("",asset =>true);
				return assetPathList;
			}
#endif

			[SerializeField]
			[ValueDropdown("EditorAssetPathList", AppendNextDrawer = true)]
			private string path;
			[SerializeField]
			private AssetLoadAPI loadAPI;
			public ResourcesKey(string path, AssetLoadAPI loadAPI)
			{
				this.path=path;
				this.loadAPI=loadAPI;
			}

			public string Path => path;
			public AssetLoadAPI LoadAPI => loadAPI;

			// 키로 사용해야하는 구조체에서 로직의 분기를 담당하는 변수를 가지고 있다는ㅠ 건 뭔가 이상함.
			// IResourcesController의 IsLoaded 를 대신 사용할 것.
			//public bool IsLoaded => isLoaded;
		}

		public bool IsLoaded(ResourcesKey resourcesKey);
		public void Load<T>(string path, AssetLoadAPI loadAPI, Action<ResourcesKey> onLoaded) where T : Object;
		public void GetAsset<T>(ResourcesKey resourcesKey, Action<T> onComplete) where T : Object;
		public void Instantiate(ResourcesKey resourcesKey, Vector3 position, Quaternion rotation, Transform parent, Action<GameObject> onComplete);
		public Awaitable<ResourcesKey> Load<T>(string path, AssetLoadAPI loadAPI) where T : Object;
		public Awaitable<T> GetAsset<T>(ResourcesKey resourcesKey) where T : Object;
		public Awaitable<GameObject> Instantiate(ResourcesKey resourcesKey, Vector3 position, Quaternion rotation, Transform parent);
		void DestroyObject(ResourcesKey loadKey, GameObject asset);

		public void Instantiate(ResourcesKey resourcesKey, Transform parent, Action<GameObject> onComplete)
		{
			Instantiate(resourcesKey, Vector3.zero, Quaternion.identity, parent, onComplete);
		}
		public async Awaitable<GameObject> Instantiate(ResourcesKey resourcesKey, Transform parent)
		{
			return await Instantiate(resourcesKey, Vector3.zero, Quaternion.identity, parent);
		}
		public void LocalInstantiate(ResourcesKey resourcesKey, Vector3 localPosition, Quaternion localRotation, Transform parent, Action<GameObject> onComplete)
		{
			if(parent != null)
			{
				localPosition = parent.TransformPoint(localPosition);
				localRotation = parent.rotation * localRotation;
			}
			Instantiate(resourcesKey, localPosition, localRotation, parent, onComplete);
		}
		public async Awaitable<GameObject> LocalInstantiate(ResourcesKey resourcesKey, Vector3 localPosition, Quaternion localRotation, Transform parent)
		{
			if(parent != null)
			{
				localPosition = parent.TransformPoint(localPosition);
				localRotation = parent.rotation * localRotation;
			}
			return await Instantiate(resourcesKey, localPosition, localRotation, parent);
		}
	}
}
