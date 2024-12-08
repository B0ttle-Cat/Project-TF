using UnityEngine;

using BC.ODCC;

namespace TFContent.Player
{
	internal class SearchPlayer : ComponentBehaviour
	{
		private QuerySystem system;
		private OdccQueryCollector collector;

		private void Log(string msg)
		{
			UnityEngine.Debug.Log($"[SearchPlayer] {msg}");
		}

		public bool Search(out Player player, int playerIdx)
		{
			Log($"Search_{playerIdx} Start");
			player = null;
			collector = OdccQueryCollector.CreateQueryCollector(system);
			foreach (var obj in collector.GetQueryItems())
			{
				if (obj.ThisContainer.TryGetData<PlayerData>(out var data))
				{
					if (data.PlayerIdx == playerIdx)
					{
						Log($"Search_{playerIdx} Success");
						player = obj as Player;
						return true;
					}
				}
			}
			Log($"Search_{playerIdx} Failed");
			return false;
		}

		protected override void BaseDestroy()
		{
			base.BaseDestroy();
			//collector.DeleteActionEvent(LOOP_EVENT_NAME);
			OdccQueryCollector.DeleteQueryCollector(system);
		}
		protected override void BaseStart()
		{
			base.BaseStart();
			system = QuerySystemBuilder.CreateQuery().WithAll<Player>().Build(ThisObject, QuerySystem.RangeType.Child);
		}
	}
}
