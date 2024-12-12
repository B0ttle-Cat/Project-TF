using BC.ODCC;
namespace TFSystem
{
	public class NetworkUser : ComponentBehaviour, INetworkUser
	{
		private UserBaseData userBaseData;
		public UserBaseData UserBaseData => userBaseData;

		private IApplicationController appController;

		protected override async void BaseAwake()
		{
			appController = ThisContainer.GetParentObject<IApplicationController>();

			userBaseData = await ThisContainer.AwaitGetData<UserBaseData>();
			gameObject.name = $"User_{userBaseData.UserIdx:00}: {userBaseData.Nickname}";
		}
	}
}