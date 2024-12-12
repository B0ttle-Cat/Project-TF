using BC.ODCC;
namespace TFSystem
{
	public interface INetworkUser : IOdccObject
	{
		UserBaseData UserBaseData { get; }


	}
}