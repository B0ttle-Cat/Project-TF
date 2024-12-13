using System.Threading;

using BC.ODCC;

using UnityEngine;

namespace TFSystem
{
	public interface ISystemController : IOdccComponent
	{
		public TSystem GetSystemState<TSystem>() where TSystem : class, IOdccObject;
		public bool TryGetSystemState<TSystem>(out TSystem getSystem) where TSystem : class, IOdccObject;
		public Awaitable<TSystem> AwaitGetSystemState<TSystem>(CancellationToken cancellationToken = default(CancellationToken)) where TSystem : class, IOdccObject;
	}
}
