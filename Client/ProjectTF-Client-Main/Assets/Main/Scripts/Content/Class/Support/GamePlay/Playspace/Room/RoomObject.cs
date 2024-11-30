using BC.ODCC;
namespace TFContent.Playspace
{
	public class RoomObject : ObjectBehaviour//, IOdccUpdate
	{
		#region ODCCFunction
		protected override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<RoomNodeData>(out _))
			{
				ThisContainer.AddData<RoomNodeData>();
			}
		}
		///Awake 대신 사용.
		protected override void BaseAwake()
		{
			if(!ThisContainer.TryGetData<RoomNodeData>(out _))
			{
				ThisContainer.AddData<RoomNodeData>();
			}
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

		}
		///Update 대신 사용
		//void IOdccUpdate.BaseUpdate()
		//{
		//	
		//}
		#endregion
	}
}