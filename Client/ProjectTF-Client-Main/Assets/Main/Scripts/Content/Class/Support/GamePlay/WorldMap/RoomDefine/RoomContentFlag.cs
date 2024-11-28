using System;

namespace TFContent
{
	[Flags]
	public enum RoomContentFlag
	{
		Empty = 0,
		StartPoint      = 1 << 0,
		EndedPoint      = 1 << 1,
		ItemBox         = 1 << 2,
		Trap            = 1 << 3,
		EnemyContact    = 1 << 4,
		WinningTarget   = 1 << 5,
	}
}
