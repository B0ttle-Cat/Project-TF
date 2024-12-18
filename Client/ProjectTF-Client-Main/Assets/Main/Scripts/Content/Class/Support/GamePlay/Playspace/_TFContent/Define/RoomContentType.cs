﻿namespace TFContent
{
	public enum RoomContentType
	{
		일반방 = 0,    // 랜덤하게 배치됨
		시작방,        // 모두가 멀리 떨어진 위치에서 시작해야 함. (플레이어 수만큼) (일반적으론 일반방이랑 동일)
		보물창고,       // 모든 시작위치에서 최대한 동일한 거리만큼 떨어져 있어야 함. (최소 1개 이상)
		일반창고,       // 랜덤하게 배치됨
		무기창고,       // 랜덤하게 배치됨
		함정방,        // 랜덤하게 배치됨
		병사방,        // 적이 스폰됨. 시작방과 보물창고를 가로막고 있으면 안됨.
		대장방,        // 적이 스폰됨. 시작방과 보물창고를 가로막고 있으면 안됨.
	}
}
