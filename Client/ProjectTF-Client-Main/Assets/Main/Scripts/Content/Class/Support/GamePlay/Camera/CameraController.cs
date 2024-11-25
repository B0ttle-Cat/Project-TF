using System;
using System.Collections.Generic;

using BC.ODCC;

using Sirenix.OdinInspector;

using Unity.Cinemachine;

using UnityEngine;

namespace TFContent
{
	public class CameraController : ObjectBehaviour
	{
		public enum CinemachineStateType
		{
			None = 0,
			StartPose,
			MainPose
		}

		public CinemachineStateType initCinemachineState;

		[Serializable]
		public struct CinemachineStateInfo
		{
			[HideLabel]
			public CinemachineStateType stateType;
			[SerializeField, ListDrawerSettings(ShowPaging = false, ShowItemCount = false, DraggableItems = false)]
			[ValueDropdown("StateCameraList", IsUniqueList = true, ExcludeExistingValuesInList = true)]
			public List<CinemachineVirtualCameraBase> stateCamera;


#if UNITY_EDITOR
			ValueDropdownList<CinemachineVirtualCameraBase> StateCameraList()
			{
				ValueDropdownList<CinemachineVirtualCameraBase> valueDropdownList = new ValueDropdownList<CinemachineVirtualCameraBase>();

				CinemachineContainer cinemachineContainer = FindAnyObjectByType<CinemachineContainer>();
				if(cinemachineContainer != null)
				{
					cinemachineContainer.GetAllCameraList().ForEach(item => valueDropdownList.Add(item.CameraName, item.CinemachineVirtualCamera));
				}
				return valueDropdownList;
			}
#endif
		}

		[SerializeField, ListDrawerSettings(ShowPaging = false, ShowItemCount = false, DraggableItems = false)]
		private List<CinemachineStateInfo> cinemachineCameraList;



		public async Awaitable OnChangeCinemachineState(CinemachineStateType viewState)
		{
			await Awaitable.NextFrameAsync();
		}
	}
}
