using UnityEngine;

using BC.Base;
using BC.ODCC;

namespace TF.Content.Character
{
	public interface ICharacterSearch
	{
		bool Search(out Character character, int idx);
	}

	internal class CharacterSearch : ComponentBehaviour, ICharacterSearch
	{
		//private const string LOOP_EVENT_NAME = "CharacterSearch";

		private QuerySystem system;
		private OdccQueryCollector collector;

		//private OdccQueryLooper looper;

		private void Log(string msg)
		{
			UnityEngine.Debug.Log($"[CharacterCreate] {msg}");
		}

		public bool Search(out Character character, int idx)
        {
			Log($"Search_{idx} Start");
			character = null;
			collector = OdccQueryCollector.CreateQueryCollector(system);
			foreach (var obj in collector.GetQueryItems())
			{
				if (obj.ThisContainer.TryGetData<CharacterData>(out var data))
				{
					if (data.idx == idx)
					{
						Log($"Search_{idx} Success");
						character = obj as Character;
						return true;
					}
				}
			}
			Log($"Search_{idx} Failed");
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
			system = QuerySystemBuilder.CreateQuery().WithAll<Character>().Build(ThisObject, QuerySystem.RangeType.Child);
			//collector = OdccQueryCollector.CreateQueryCollector(system);
			//looper = collector.CreateActionEvent(LOOP_EVENT_NAME).CallForeach<Character>(
			//	(loopInfo, a) =>
			//	{
			//	});
		}
	}
}
