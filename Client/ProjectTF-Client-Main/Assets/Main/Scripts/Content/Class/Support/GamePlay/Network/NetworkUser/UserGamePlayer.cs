using BC.ODCC;
namespace TFContent
{
	public class UserGamePlayer : ComponentBehaviour//, IOdccUpdate
	{
		private UserGamePlayData userPlayData;

		private ObjectBehaviour CurrentMapObject;
		private ObjectBehaviour CharacterObject;



		#region ODCCFunction
		///Awake 대신 사용.
		protected override void BaseAwake()
		{

		}
		///OnEnable 대신 사용.
		protected override async void BaseEnable()
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

		}
		///Update 대신 사용
		//void IOdccUpdate.BaseUpdate()
		//{
		//	
		//}
		#endregion
	}
}