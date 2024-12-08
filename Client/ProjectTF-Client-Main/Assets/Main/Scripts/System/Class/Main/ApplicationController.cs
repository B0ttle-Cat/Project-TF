using BC.ODCC;

using TFSystem.Network;

using UnityEngine;
namespace TFSystem
{
	public class ApplicationController : ObjectBehaviour, IApplicationController
	{
		public IDataCarrier DataCarrier { get; private set; }
		public ISceneController SceneController { get; private set; }
		public ISystemController SystemController { get; private set; }
		public IResourcesController ResourcesController { get; private set; }
		public INetworkController NetworkController { get; private set; }

		[SerializeField, Space]
		private ISceneController.SceneState AppStartState = ISceneController.SceneState.MainMenuState;
#if UNITY_EDITOR
		public ISceneController.SceneState EditorOnly_AppStartState { get => AppStartState; set => AppStartState = value; }
#endif

		protected override void BaseAwake()
		{
			DataCarrier = ThisContainer.TryGetData<UniversalDataCarrier>(out var dataCarrier)
				? dataCarrier : ThisContainer.AddData<UniversalDataCarrier>();

			SceneController = ThisContainer.TryGetComponent<SceneController>(out var sceneController)
				? sceneController : ThisContainer.AddComponent<SceneController>();

			SystemController = ThisContainer.TryGetComponent<SystemController>(out var systemController)
				? systemController : ThisContainer.AddComponent<SystemController>();

			ResourcesController = ThisContainer.TryGetComponent<ResourcesController>(out var resourcesController)
				? resourcesController : ThisContainer.AddComponent<ResourcesController>();

			NetworkController = ThisContainer.TryGetComponent<NetworkController>(out var networkController)
				? networkController : ThisContainer.AddComponent<NetworkController>();
		}
		protected override void BaseStart()
		{
			SceneController.ChangeSceneState(AppStartState, null);
		}

	}
}