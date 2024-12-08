using System.Collections.Generic;

using UnityEngine;

using BC.ODCC;

namespace TFContent.Character
{
	internal class ItemBoxSearch : ComponentBehaviour
	{
		private QuerySystem system;
		private OdccQueryCollector collector;

		public bool SearchAll(out List<BoxObject> outputs)
		{
			outputs = new List<BoxObject>();
			foreach (var obj in collector.GetQueryItems())
			{
				if (obj is BoxObject box)
				{
					outputs.Add(box);
				}
			}
			return 0 < outputs.Count;
		}

		public bool Search(out BoxObject output, Vector2Int point)
		{
			output = null;
			foreach (var obj in collector.GetQueryItems())
			{
				if (obj is BoxObject box)
				{
					if (box.point == point)
					{
						output = box;
						return true;
					}
				}
			}
			return false;
		}

		protected override void BaseDestroy()
		{
			base.BaseDestroy();
			OdccQueryCollector.DeleteQueryCollector(system);
		}
		protected override void BaseStart()
		{
			base.BaseStart();
			system = QuerySystemBuilder.CreateQuery().WithAll<BoxObject>().Build(ThisObject, QuerySystem.RangeType.Child);
			collector = OdccQueryCollector.CreateQueryCollector(system);
		}
	}
}
