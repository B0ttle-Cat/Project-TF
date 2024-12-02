using BC.ODCC;
namespace TFContent.Playspace
{
	public class RoomNode : ComponentBehaviour//, IOdccUpdate
	{
		#region ODCCFunction
		///Awake 대신 사용.
		protected override void BaseAwake()
		{

		}
		///OnEnable 대신 사용.
		protected override void BaseEnable()
		{
			if(ThisContainer.TryGetData<NodeLinkData>(out var nodeData))
			{
				var linkInfo = nodeData.nodeLink;

				if(linkInfo.linkIndex == -1)
				{
					if(ThisContainer.TryGetAllComponentInChild<RoomPlane>(out var plane))
					{
						plane.ForEach(plane => plane.gameObject.SetActive(false));
					}
					if(ThisContainer.TryGetAllComponentInChild<RoomWall>(out var walls))
					{
						walls.ForEach(wall => wall.gameObject.SetActive(true));
					}
				}
				else
				{
					if(ThisContainer.TryGetAllComponentInChild<RoomPlane>(out var plane))
					{
						plane.ForEach(plane => plane.gameObject.SetActive(true));
					}
					if(ThisContainer.TryGetAllComponentInChild<RoomWall>(out var walls))
					{
						walls.ForEach(wall => wall.gameObject.SetActive(false));
					}
				}
			}
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