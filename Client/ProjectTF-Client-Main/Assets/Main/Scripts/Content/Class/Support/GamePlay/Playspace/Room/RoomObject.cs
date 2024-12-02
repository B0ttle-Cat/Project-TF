using BC.ODCC;

using Sirenix.OdinInspector;
namespace TFContent.Playspace
{
	public class RoomObject : ObjectBehaviour//, IOdccUpdate
	{
		[ShowInInspector]
		private QuerySystem nodeQuerySystem;
		[ShowInInspector]
		private OdccQueryCollector queryCollector;

		[ShowInInspector,ReadOnly]
		private RoomNodeData roomNodeData;


		#region ODCCFunction
		///Awake 대신 사용.
		protected override void BaseAwake()
		{

		}
		///OnDestroy 대신 사용
		protected override void BaseDestroy()
		{
			if(queryCollector != null)
			{
				OdccQueryCollector.DeleteQueryCollector(nodeQuerySystem);
			}
		}

		///Start 대신 사용.
		protected override async void BaseStart()
		{
			roomNodeData = await ThisContainer.AwaitGetData<RoomNodeData>();

			nodeQuerySystem = QuerySystemBuilder.CreateQuery()
				.WithAll<RoomElement, RoomNode, NodeLinkData>().Build(this, QuerySystem.RangeType.Child);

			queryCollector = OdccQueryCollector.CreateQueryCollector(nodeQuerySystem, this)
				.CreateChangeListEvent(UpdateNodeItem)
				.GetCollector();
		}
		private void UpdateNodeItem(ObjectBehaviour item, bool isAdd)
		{
			if(item == null) return;
			if(!item.TryGetComponent<RoomNode>(out var roomNode)) return;
			if(!item.ThisContainer.TryGetData<NodeLinkData>(out var nodeLinkData)) return;

			if(isAdd)
			{
				nodeLinkData.nodeIndex = roomNodeData.nodeIndex;
				nodeLinkData.tableIndex = roomNodeData.tableIndex;

				int length = roomNodeData.linkList.Length;
				for(int i = 0 ; i < length ; i++)
				{
					if(roomNodeData.linkList[i].linkDir == nodeLinkData.nodeLink.linkDir)
					{
						nodeLinkData.nodeLink = roomNodeData.linkList[i];
					}
				}
				roomNode.enabled = false;
				roomNode.enabled = true;
			}
			else
			{

			}
		}
		///Update 대신 사용
		//void IOdccUpdate.BaseUpdate()
		//{
		//	
		//}
		#endregion
	}
}