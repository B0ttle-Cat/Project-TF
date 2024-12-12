using System.Collections.Generic;

using BC.ODCC;
namespace TFContent
{
	public interface IOnlineRoomUserListUpdate : IOdccComponent
	{
		void OnUserListUpdate(List<(int userIdx, string nickname)> userList);
		void OnEnterUser(int userIdx, string nickname);
		void OnLeaveUser(int userIdx);
	}

}