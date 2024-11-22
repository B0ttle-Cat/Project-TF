using System;
using System.Collections.Generic;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using Unity.Cinemachine;

using UnityEngine;

namespace TF.Content
{
	public class CameraContainer : ComponentBehaviour
	{
		[Serializable]
		private struct CameraObject
		{
			private string _name;
			private Camera camera;
			private CinemachineBrain cinemachineBrain;
			[ShowInInspector, HorizontalGroup("Camera"), HideLabel]
			public Camera ThisCamera { get => camera; set => camera = value; }
			[ShowInInspector, HorizontalGroup("Camera"), HideLabel]
			public string CameraName { get => (_name.IsNullOrWhiteSpace() ? (camera == null ? "" : camera.name) : _name); set => _name = value; }

			[ShowInInspector, HideLabel]
			[HorizontalGroup("CinemachineBrain")]
			public CinemachineBrain CinemachineBrain {
				get {
					if(camera == null)
					{
						cinemachineBrain = null;
					}
					else if(cinemachineBrain == null)
					{
						cinemachineBrain = camera.GetComponent<CinemachineBrain>();
					}
					return cinemachineBrain;
				}
				set {
					cinemachineBrain = value;
				}
			}
			[HorizontalGroup("CinemachineBrain")]
			[ShowInInspector, HideLabel, ShowIf("ShowCinemachineBrainRemoveButton")]
			public OutputChannels ChannelMask {
				get {
					if(cinemachineBrain == null) return 0;
					else return cinemachineBrain.ChannelMask;
				}
				set {
					if(cinemachineBrain == null) return;
					else cinemachineBrain.ChannelMask = value;
				}
			}
#if UNITY_EDITOR
			private bool ShowCinemachineBrainAddButton => ThisCamera != null && CinemachineBrain == null;
			private bool ShowCinemachineBrainRemoveButton => CinemachineBrain != null;
			[HorizontalGroup("CinemachineBrain", width: 25)]
			[Button("", ButtonSizes.Medium, Icon = SdfIconType.PlusSquareFill), ShowIf("ShowCinemachineBrainAddButton")]
			private void AddCinemachineBrain()
			{
				cinemachineBrain = camera.gameObject.AddComponent<CinemachineBrain>();
				cinemachineBrain.ChannelMask = OutputChannels.Default;
			}
			[HorizontalGroup("CinemachineBrain", width: 25)]
			[Button("", ButtonSizes.Medium, Icon = SdfIconType.XSquare), ShowIf("ShowCinemachineBrainRemoveButton")]
			private void RemoveCinemachineBrain()
			{
				if(cinemachineBrain != null) DestroyImmediate(cinemachineBrain);
			}
#endif
		}

		[SerializeField]
		private List<CameraObject> CameraList;


		public bool TryFindCamera(string cameraName, out Camera camera)
		{
			camera = null;
			if(cameraName.IsNullOrWhiteSpace()) return false;

			int findIndex = CameraList.FindIndex((x) => x.CameraName == cameraName);
			if(findIndex < 0) return false;

			camera = CameraList[findIndex].ThisCamera;

			return camera != null;
		}
	}
}
