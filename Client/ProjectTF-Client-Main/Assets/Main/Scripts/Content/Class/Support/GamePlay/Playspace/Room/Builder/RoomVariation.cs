using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace TFContent.Playspace
{
	public class RoomVariation : ComponentBehaviour//, IOdccUpdate
	{
		readonly static float realModelScale = 4f/5f;
		readonly static int[] wallModelSize = new int[3]{1,2,4};

		public RoomResourcesData roomResourcesData;

		public FloorVariationRawData floorVariationRawData;
		public WallVariationRawData wallVariationRawDataX;
		public WallVariationRawData wallVariationRawDataIX;
		public WallVariationRawData wallVariationRawDataY;
		public WallVariationRawData wallVariationRawDataIY;

		[Button]
		public void StartRoomVariation()
		{
			if(!ThisContainer.TryGetData<RoomVariationData>(out var roomVar)) return;

			var oldState = Random.state;
			Random.InitState(roomVar.roomRandomSeed);

			FloorVariation();
			WallVariation();
			Random.state = oldState;

			void FloorVariation()
			{
				floorVariationRawData = new FloorVariationRawData(new Vector2Int(4, 4));
				floorVariationRawData.SetRandomFloor(2, 4);
			}
			void WallVariation()
			{
				int doorModelSize = 4;
				int doorVariationID = -1;
				int[] wallModelSize = new int[3] { 1, 2, 4 };
				Vector2Int roomFloorSize = floorVariationRawData.roomFloorSize;

				int roomLengthX = roomFloorSize.x;
				int roomLengthY = roomFloorSize.y;

				wallVariationRawDataX = new WallVariationRawData(roomLengthX);
				wallVariationRawDataIX = new WallVariationRawData(roomLengthX);
				wallVariationRawDataY = new WallVariationRawData(roomLengthY);
				wallVariationRawDataIY = new WallVariationRawData(roomLengthY);

				wallVariationRawDataX.SetDoor(doorModelSize, doorVariationID);
				wallVariationRawDataIX.SetDoor(doorModelSize, doorVariationID);
				wallVariationRawDataY.SetDoor(doorModelSize, doorVariationID);
				wallVariationRawDataIY.SetDoor(doorModelSize, doorVariationID);

				wallVariationRawDataX.SplitWall(wallModelSize);
				wallVariationRawDataIX.SplitWall(wallModelSize);
				wallVariationRawDataY.SplitWall(wallModelSize);
				wallVariationRawDataIY.SplitWall(wallModelSize);

				wallVariationRawDataX.PrintWall();
				wallVariationRawDataIX.PrintWall();
				wallVariationRawDataY.PrintWall();
				wallVariationRawDataIY.PrintWall();
			}
		}

		[Button]
		public async void StartRoomInstantiate()
		{
			if(!ThisContainer.TryGetData<RoomTransformData>(out var roomTransfromData)) return;

			WorldMapSystem worldMapSystem = await ThisContainer.AwaitGetParentObject<WorldMapSystem>(null,DestroyCancelToken);
			if(worldMapSystem == null) return;

			var IResources = worldMapSystem.AppController.ResourcesController;



			FloorInstantiate();
			WallInstantiate();

			void FloorInstantiate()
			{
				//IResources.Instantiate()
			}
			void WallInstantiate()
			{

			}
		}
	}
}