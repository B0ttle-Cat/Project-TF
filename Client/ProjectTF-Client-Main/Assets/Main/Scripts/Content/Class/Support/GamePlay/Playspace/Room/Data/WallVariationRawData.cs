using System;
using System.Collections.Generic;
using System.Linq;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace TFContent.Playspace
{
	[Serializable]
	public struct WallVariationRawData
	{
		[ShowInInspector, ReadOnly]
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
		public void SplitWall(int[] wallModelSize)
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

		public List<(int, int, int)> GetWallList()
		{
			List<(int, int,int)> list = new List<(int ,int, int)>();

			int length = wallArray.Length;
			if(length==0) return list;

			int id = wallArray[0];
			int start = 0;
			int size = 0;
			for(int i = 0 ; i < length ; i++)
			{
				if(id == wallArray[i])
				{
					size++;
				}
				else
				{
					list.Add((id, start, size));
					id = wallArray[i];
					start = i;
					size = 1;
				}
			}
			list.Add((id, start, size));
			return list;
		}
	}
}
