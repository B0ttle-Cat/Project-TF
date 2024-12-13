using System.Threading;

using BC.Base;
using BC.ODCC;

using UnityEngine;

namespace TFSystem
{
	public class SystemController : ComponentBehaviour, ISystemController
	{
		[UnityEngine.SerializeField]
		private OdccQueryCollector systemStateCollector;

		protected override void BaseAwake()
		{
			base.BaseAwake();

			systemStateCollector = OdccQueryCollector.CreateQueryCollector(QuerySystemBuilder.SimpleQueryBuild<SystemState>()).GetCollector();
		}

		TSystem GetSystemState<TSystem>() where TSystem : class, IOdccObject
		{
			var list = systemStateCollector.GetQueryItems();
			foreach(var item in list)
			{
				var getSystem = item.ThisContainer.GetObject<TSystem>();
				if(getSystem != null) return getSystem;
			}
			return null;
		}
		TSystem ISystemController.GetSystemState<TSystem>()
		{
			return GetSystemState<TSystem>();
		}

		bool ISystemController.TryGetSystemState<TSystem>(out TSystem getSystem)
		{
			getSystem = GetSystemState<TSystem>();
			return getSystem != null;
		}

		async Awaitable<TSystem> ISystemController.AwaitGetSystemState<TSystem>(CancellationToken cancellationToken)
		{
			return await AwaitableUtility.WaitNotNull<TSystem>(() => GetSystemState<TSystem>(), cancellationToken);
		}
	}
}
