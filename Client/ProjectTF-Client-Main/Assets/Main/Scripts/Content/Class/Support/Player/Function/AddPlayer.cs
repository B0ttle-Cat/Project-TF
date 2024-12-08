using System;

using UnityEngine;

using BC.ODCC;
using TFSystem;

namespace TFContent.Player
{
	public interface IAddPlayer
	{
		bool Add(int playerIdx, Action<Player> success, Action failed = null);
	}

	internal class AddPlayer : ComponentBehaviour
	{
		private const string EMPTY_OBJECT_PATH = "Prefabs/Empty";

		private IResourcesController resources;
		private Transform parent = null;

		private void Log(string msg)
		{
			UnityEngine.Debug.Log($"[AddPlayer] {msg}");
		}

		public bool Add(int playerIdx, Action<Player> success, Action failed = null)
		{
			Log($"Add_{playerIdx} Start");
			if (ThisContainer.TryGetComponent<SearchPlayer>(out var search))
			{
				if (search.Search(out var player, playerIdx))
				{
					Log($"Add_{playerIdx} Search Success");
					success?.Invoke(player);
					return true;
				}
			}
			if (resources != null)
			{
				Log($"Add_{playerIdx} GetObject");
				resources.Load<GameObject>(EMPTY_OBJECT_PATH, IResourcesController.AssetLoadAPI.ResourcesAPI,
					(key) =>
					{
						Load_Complete(key, GetData(playerIdx), success, failed);
					});
			}
			else
			{
				Log($"Add_{playerIdx} Failed");
				failed?.Invoke();
			}
			return false;

			// ================================================================

			PlayerData GetData(int playerIdx)
			{
				return new PlayerData(playerIdx);
			}

			void Load_Complete(IResourcesController.ResourcesKey key, PlayerData data, Action<Player> success, Action failed)
			{
				resources.Instantiate(key, parent, (obj) => Create_Complete(obj, data, success, failed));
			}

			void Create_Complete(GameObject obj, PlayerData data, Action<Player> success, Action failed)
			{
				if (obj == null)
				{
					Log($"Add_{data.PlayerIdx} Failed :: Object Null");
					failed?.Invoke();
					return;
				}
				Player player = null;
				if (!obj.TryGetComponent(out player))
				{
					player = obj.AddComponent<Player>();
				}
				obj.name = GetObjectName(data.PlayerIdx);
				player.SetPlayerData(data);

				Log($"Add_{data.PlayerIdx} Success");
				success?.Invoke(player);
			}

			string GetObjectName(int idx)
			{
				return $"Player_{idx}";
			}
		}

		protected override void BaseStart()
		{
			base.BaseStart();
			if (ThisContainer.TryGetObject<PlayerSystem>(out var system))
			{
				resources = system.AppController.ResourcesController;
			}
		}

		protected override void BaseAwake()
		{
			base.BaseAwake();
			parent = ThisObject.transform;
		}
	}
}
