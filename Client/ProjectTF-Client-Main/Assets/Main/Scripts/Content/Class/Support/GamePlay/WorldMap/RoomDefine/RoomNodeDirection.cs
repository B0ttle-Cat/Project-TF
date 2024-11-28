using System;

namespace TFContent
{
	[Flags]
	public enum RoomNodeDirectionFlag
	{
		None = 0,
		TR_Direction = 1<<0,
		TL_Direction = 1<<1,
		BL_Direction = 1<<2,
		BR_Direction = 1<<3,
		All_Direction = TR_Direction | TL_Direction | BL_Direction | BR_Direction,
	}
}
