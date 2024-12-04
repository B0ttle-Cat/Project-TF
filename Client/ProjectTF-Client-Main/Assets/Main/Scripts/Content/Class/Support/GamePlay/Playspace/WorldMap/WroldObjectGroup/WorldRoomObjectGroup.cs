using BC.ODCC;

using Sirenix.OdinInspector;
namespace TFContent.Playspace
{
	public class WorldRoomObjectGroup : ComponentBehaviour//, IOdccUpdate
	{
		#region ODCCFunction
		///Awake 대신 사용.
		[ShowInInspector, ReadOnly]
		private QuerySystem roomObjectQuery;
		[ShowInInspector, ReadOnly]
		private OdccQueryCollector roomObjectCollector;
		protected override void BaseAwake()
		{
			roomObjectQuery = QuerySystemBuilder.CreateQuery().WithAll<RoomObject>().Build();
			roomObjectCollector = OdccQueryCollector.CreateQueryCollector(roomObjectQuery)
				.CreateChangeListEvent(UpdateRoomObject)
				.GetCollector();
		}

		private void UpdateRoomObject(ObjectBehaviour item, bool isAdd)
		{
			if(item is not RoomObject roomObject) return;
			if(roomObject == null) return;
			if(isAdd)
			{
				CreateRoomObject(roomObject);
			}
			else
			{
				DestroyRoomObject(roomObject);
			}
		}

		private void CreateRoomObject(RoomObject roomObject)
		{

		}
		private void DestroyRoomObject(RoomObject roomObject)
		{

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
			if(roomObjectCollector != null)
			{
				roomObjectCollector.DeleteChangeListEvent(UpdateRoomObject);
				OdccQueryCollector.DeleteQueryCollector(roomObjectQuery);
				roomObjectCollector = null;
			}
			roomObjectQuery = null;
		}
		///Update 대신 사용
		//void IOdccUpdate.BaseUpdate()
		//{
		//	
		//}
		#endregion
	}
}