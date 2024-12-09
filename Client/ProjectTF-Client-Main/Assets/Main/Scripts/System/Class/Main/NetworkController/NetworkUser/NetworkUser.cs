using BC.ODCC;
namespace TFSystem
{
	public class NetworkUser : ComponentBehaviour, INetworkUser
	{
		private UserBaseData userBaseData;
		public UserBaseData UserBaseData => userBaseData;

		protected override void BaseAwake()
		{
			ThisContainer.TryGetData(out userBaseData);
		}
	}
}