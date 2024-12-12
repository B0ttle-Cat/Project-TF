using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

namespace TFContent
{
	[Serializable]
	public struct RoomContentCreateData
	{
		[ValueDropdown("FindAllRoomThemeTables_ValueDropdownList", AppendNextDrawer = true)]
		public string roomThemeName;

		public List<ContentPoint> contentPoint;
		[Serializable]
		public struct ContentPoint
		{
			public RoomContentType contentType;
			[LabelText("랜덤 생성 점수")]
			public int point;
			[LabelText("최소 생성 개수")]
			public int minCount;
		}

#if UNITY_EDITOR
		private ValueDropdownList<string> FindAllRoomThemeTables_ValueDropdownList() => RoomDefine.FindAllRoomThemeTables_ValueDropdownList();
#endif
	}
}
