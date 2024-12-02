using BC.ODCC;
namespace TFSystem
{
	public interface INetworkController : IOdccComponent
	{
		int ThisUserIndex { get; set; }
		void OnConnectAsync();
	}
}