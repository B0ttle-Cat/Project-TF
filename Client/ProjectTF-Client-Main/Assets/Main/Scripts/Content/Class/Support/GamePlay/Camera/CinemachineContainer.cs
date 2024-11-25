using System;
using System.Collections.Generic;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using Unity.Cinemachine;

using UnityEngine;

namespace TFContent
{
	public class CinemachineContainer : ComponentBehaviour
	{
		[Serializable]
		public struct CameraObject
		{
			private string _name;
			private CinemachineVirtualCameraBase cinemachineVirtualCamera;
			[ShowInInspector, HorizontalGroup("Camera"), HideLabel]
			public string CameraName { get => (_name.IsNullOrWhiteSpace() ? (cinemachineVirtualCamera == null ? "" : cinemachineVirtualCamera.name) : _name); set => _name = value; }

			[ShowInInspector, HideLabel]
			[HorizontalGroup("CinemachineBrain")]
			public CinemachineVirtualCameraBase CinemachineVirtualCamera {
				get {
					return cinemachineVirtualCamera;
				}
				set {
					cinemachineVirtualCamera = value;
				}
			}
			[HorizontalGroup("CinemachineBrain")]
			[ShowInInspector, HideLabel, ShowIf("ShowOutputChannel")]
			public OutputChannels OutputChannels {
				get {
					if(cinemachineVirtualCamera == null) return 0;
					else return cinemachineVirtualCamera.OutputChannel;
				}
				set {
					if(cinemachineVirtualCamera == null) return;
					else cinemachineVirtualCamera.OutputChannel = value;
				}
			}
#if UNITY_EDITOR
			private bool ShowOutputChannel => cinemachineVirtualCamera != null;
#endif
		}

		[SerializeField]
		private List<CameraObject> CameraList;


		public bool TryFindCamera(string cameraName, out CinemachineVirtualCameraBase camera)
		{
			camera = null;
			if(cameraName.IsNullOrWhiteSpace()) return false;

			int findIndex = CameraList.FindIndex((x) => x.CameraName == cameraName);
			if(findIndex < 0) return false;

			camera = CameraList[findIndex].CinemachineVirtualCamera;

			return camera != null;
		}
		public List<CameraObject> GetAllCameraList()
		{
			return CameraList;
		}
	}
}
