using BC.ODCC;

namespace TF.System
{
	public interface ISystemController : IOdccComponent
	{
		public TSystem GetSystemState<TSystem>() where TSystem : SystemState;
		public bool TryGetSystemState<TSystem>(out TSystem getSystem) where TSystem : SystemState;
	}
}
