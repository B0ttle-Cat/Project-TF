using UnityEngine;

using BC.Base;
using BC.ODCC;
using System.Collections.Generic;

namespace TFContent.Character
{
	public interface ICharacterSearch : IOdccComponent
	{
		bool Search_Character(out Character character, int idx);
		bool Search_ItemBox(out CharacterItemBox itemBox);
	}

	internal class CharacterSearch : ComponentBehaviour, ICharacterSearch
	{
		private Dictionary<string, (QuerySystem, OdccQueryCollector)> queryList = new Dictionary<string, (QuerySystem, OdccQueryCollector)>();

		private void Log(string msg)
		{
			UnityEngine.Debug.Log($"[CharacterCreate] {msg}");
		}

		private bool GetQuery(out OdccQueryCollector collector, string key)
		{
			collector = null;
			if (queryList.ContainsKey(key))
			{
				collector = queryList[key].Item2;
				return true;
			}
			return false;
		}

		public bool Search_Character(out Character character, int idx)
        {
			Log($"Search_{idx} Start");
			character = null;
			if (GetQuery(out var collector, "Character"))
			{
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
			}
			Log($"Search_{idx} Failed");
			return false;
        }

		public bool Search_ItemBox(out CharacterItemBox itemBox)
		{
			itemBox = null;
			if (GetQuery(out var collector, "CharacterItemBox"))
			{
				foreach (var obj in collector.GetQueryItems())
				{
					if (obj is CharacterItemBox target)
					{
						itemBox = target;
						return true;
					}
				}
			}
			return false;
		}

		protected override void BaseDestroy()
		{
			base.BaseDestroy();
			foreach (var query in queryList)
			{
				OdccQueryCollector.DeleteQueryCollector(query.Value.Item1);
			}
		}
		protected override void BaseStart()
		{
			base.BaseStart();
			queryList.Clear();
			SetQuery<Character>("Character");
			SetQuery<CharacterItemBox>("CharacterItemBox");

			void SetQuery<T>(string key) where T : ObjectBehaviour
			{
				var system = QuerySystemBuilder.CreateQuery().WithAll<T>().Build(ThisObject, QuerySystem.RangeType.Child);
				var collector = OdccQueryCollector.CreateQueryCollector(system);
				queryList.Add(key, (system, collector));
			}
		}
	}
}
