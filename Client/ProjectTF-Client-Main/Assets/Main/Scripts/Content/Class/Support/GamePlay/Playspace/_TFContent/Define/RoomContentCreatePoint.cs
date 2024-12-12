using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

namespace TFContent
{
	[Serializable]
	public struct RoomContentCreatePoint
	{
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
	}
}
