using BC.ODCC;
namespace TFSystem
{
	public interface INetworkUser : IOdccComponent
	{
		UserBaseData UserBaseData { get; }


	}
}