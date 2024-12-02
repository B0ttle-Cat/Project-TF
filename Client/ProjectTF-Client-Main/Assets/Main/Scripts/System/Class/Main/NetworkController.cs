using BC.ODCC;
namespace TFSystem
{
	public class NetworkController : ComponentBehaviour, INetworkController
	{
		int thisUserIndex;

		protected override void BaseAwake()
		{
			thisUserIndex = -1;
		}
		///OnEnable 대신 사용.
		protected override void BaseEnable()
		{

		}
		///Start 대신 사용.
		protected override void BaseStart()
		{

		}
		///OnDisable 대신 사용.
		protected override void BaseDisable()
		{

		}
		///OnDestroy 대신 사용
		protected override void BaseDestroy()
		{
			thisUserIndex = -1;
		}
		private void ConnectAsync()
		{

		}

		int INetworkController.ThisUserIndex { get => thisUserIndex; set => thisUserIndex = value; }
		void INetworkController.OnConnectAsync() => ConnectAsync();
	}
}