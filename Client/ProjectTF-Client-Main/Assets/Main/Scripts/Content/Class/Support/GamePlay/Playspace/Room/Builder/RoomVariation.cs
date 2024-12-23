using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace TFContent.Playspace
{
	public class RoomVariation : ComponentBehaviour//, IOdccUpdate
	{
		#region ODCCFunction
		///Awake 대신 사용.
		protected override void BaseAwake()
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

		}
		///Update 대신 사용
		//void IOdccUpdate.BaseUpdate()
		//{
		//	
		//}
		#endregion

		[Button]
		public void WallVariation()
		{
			if(!ThisContainer.TryGetData<RoomVariationData>(out var roomVar)) return;

			int roomRandomSeed = roomVar.roomRandomSeed;

			Random.InitState(roomRandomSeed);

			float realModelScale = 4f/5f;

			// 2~3 싸이즈
			Vector2Int roomFloorSize = new Vector2Int(Random.Range(2,4),Random.Range(2,4));

			int floorModelSize = 4;
			int doorModelSize = 4;
			int doorVariationID = -1;
			int[] wallModelSize = new int[3]{1,2,4};
			Vector2Int roomSize = roomFloorSize * floorModelSize;

			int roomLengthX = roomSize.x;
			int roomLengthY = roomSize.y;

			WallVariationRawData wallLine1 = new WallVariationRawData(roomLengthX);
			WallVariationRawData wallLine2 = new WallVariationRawData(roomLengthX);
			WallVariationRawData wallLine3 = new WallVariationRawData(roomLengthY);
			WallVariationRawData wallLine4 = new WallVariationRawData(roomLengthY);

			wallLine1.SetDoor(doorModelSize, doorVariationID);
			wallLine2.SetDoor(doorModelSize, doorVariationID);
			wallLine3.SetDoor(doorModelSize, doorVariationID);
			wallLine4.SetDoor(doorModelSize, doorVariationID);

			wallLine1.SetRandomWall(wallModelSize);
			wallLine2.SetRandomWall(wallModelSize);
			wallLine3.SetRandomWall(wallModelSize);
			wallLine4.SetRandomWall(wallModelSize);

			wallLine1.PrintWall();
			wallLine2.PrintWall();
			wallLine3.PrintWall();
			wallLine4.PrintWall();
		}

		[Serializable]
		public struct WallVariationRawData
		{
			private int length;
			public int[] wallArray;
			public WallVariationRawData(int length)
			{
				this.length = length;
				wallArray = new int[length];
			}

			public void SetDoor(int doorSize, int id)
			{
				if(id == 0) return;
				if(length < doorSize) return;

				int center = length/2;
				int doorStart = (center - doorSize/2);
				int doorEnded = (doorStart + doorSize);

				if(id > 0) id = -id;

				for(int i = doorStart ; i < doorEnded ; i++)
				{
					wallArray[i] = id;
				}
			}
			public void SetRandomWall(int[] wallModelSize)
			{
				int id = 0;
				do
				{
					(int startIndex, int emptySize) = GetWallEmptySizeWithStart();
					if(startIndex < 0 || emptySize == 0) break;

					var randomList = wallModelSize.Where(i=>i<=emptySize).ToList();
					int count = randomList.Count();
					var randomSize = randomList.ElementAt(Random.Range(0, count));
					SetWall(startIndex, randomSize, ++id);
				}
				while(id < 100);
				if(id == 100)
				{
					Debug.LogError("SetRandomWall 무한루프타고있음");
				}
			}
			private (int, int) GetWallEmptySizeWithStart()
			{
				int isStart = -1;
				int emptyCount = 0;
				for(int i = 0 ; i < length ; i++)
				{
					if(isStart < 0 && wallArray[i] == 0)
					{
						isStart = i;
						emptyCount=1;
					}
					else if(isStart >= 0 && wallArray[i] != 0)
					{
						return (isStart, emptyCount);
					}
					else if(isStart >= 0)
					{
						emptyCount++;
					}
				}
				return (isStart, emptyCount);
			}
			private void SetWall(int start, int size, int id)
			{
				if(start <0) start = 0;
				for(int i = start ; i < start + size ; i++)
				{
					if(wallArray.Length <= i) break;
					wallArray[i] = id;
				}
			}

			public void PrintWall()
			{
				Debug.Log($"| {string.Join(" | ", wallArray)} |");
			}
		}
	}
}