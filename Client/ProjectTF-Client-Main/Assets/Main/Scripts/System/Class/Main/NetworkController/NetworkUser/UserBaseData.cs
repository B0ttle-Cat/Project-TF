using BC.ODCC;
namespace TFSystem
{
	public class UserBaseData : DataObject
	{
		public UserBaseData() : base()
		{

		}
		public bool IsLocal;
		public bool IsRemote => !IsLocal;
		public int UserIdx;
		public string Nickname;
		public NetworkStateType NetworkState;
		public enum NetworkStateType
		{
			Disconnect,
			Connect,
		}
		protected override void Disposing()
		{

		}
	}
}