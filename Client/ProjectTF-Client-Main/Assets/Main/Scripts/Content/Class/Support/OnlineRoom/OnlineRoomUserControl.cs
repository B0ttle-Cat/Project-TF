using BC.ODCC;

using TFSystem;
using TFSystem.Network;

using UnityEngine;
namespace TFContent
{
	public class OnlineRoomUserControl : ComponentBehaviour//, IOdccUpdate
	{
		[SerializeField]
		private OnlineRoomViewModel viewModel;
		private INetworkAPI.UserGroupAPI userGroupAPI;
		///Awake 대신 사용.
		protected override void BaseAwake()
		{
			if(ThisContainer.TryGetObject<SystemState>(out var state))
			{
				userGroupAPI = state.AppController.NetworkController.UserGroupAPI;
			}
		}
		///OnEnable 대신 사용.
		protected override async void BaseEnable()
		{
			await OnSnapshot();
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
		private async Awaitable OnSnapshot()
		{

		}
		private void UserEnterNty(S2C_TEMP_CHATROOM_ENTER_NTY packet)
		{
		}
		private void UserLeaveNty(S2C_TEMP_CHATROOM_LEAVE_NTY packet)
		{
		}
	}
}