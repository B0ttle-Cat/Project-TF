﻿using TFSystem;
using TFSystem.Network;

using UnityEngine;

namespace TFContent
{
	public class OnlineLobbySystem : SystemState
	{
		IUIViewController<OnlineLobbyViewState> viewController;
		protected override void AwakeOnSystem()
		{
			ThisContainer.TryGetChildObject(out viewController);
		}
		protected override void DestroyOnSystems()
		{
			viewController = null;
		}
		async protected override Awaitable StartWaitSystem()
		{
			await viewController.OnChangeViewState(OnlineLobbyViewState.OnlineLobbyDefaultState);
		}

		async protected override Awaitable EndedWaitSystem()
		{
			await viewController.OnChangeViewState(OnlineLobbyViewState.None);
		}

		public override async void ChangeSceneState(ISceneController.SceneState mainMenuState)
		{
			if(mainMenuState == ISceneController.SceneState.OnlineRoomState)
			{
				bool connect = await AppController.NetworkController.OnConnectAsync();
				if(!connect) return;

				string userNickname = AppController.NetworkController.UserNickname;

				var receive = await PacketAsyncItem.OnSendReceiveAsync<S2C_TEMP_CHATROOM_ENTER_ACK>(
					new C2S_TEMP_CHATROOM_ENTER_REQ {nickname = userNickname}
				);

				if(receive == null) return;
				if(receive.Failure) return;

				AppController.DataCarrier.ClearData()
					.AddData("nickname", receive.nickname)
					.AddData("userIdx", receive.userIdx)
					;

				base.ChangeSceneState(mainMenuState);
			}
		}
	}
}
