using BC.ODCC;

namespace TF.System
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

		public TSystem GetSystemState<TSystem>() where TSystem : SystemState
		{
			var list = systemStateCollector.GetQueryItems();
			foreach(var item in list)
			{
				var getSystem = item.ThisContainer.GetObject<TSystem>();
				if(getSystem != null) return getSystem;
			}
			return null;
		}

		public bool TryGetSystemState<TSystem>(out TSystem getSystem) where TSystem : SystemState
		{
			getSystem = GetSystemState<TSystem>();
			return getSystem != null;
		}
	}
}
