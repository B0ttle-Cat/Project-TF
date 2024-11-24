using Sirenix.OdinInspector;

using UnityEngine;

namespace TF.System
{
	public class ResourcesState : MonoBehaviour
	{
		private IResourcesController resourcesController;
		[ReadOnly]
		private IResourcesController.ResourcesKey loadKey;

		public void Init(IResourcesController resourcesController, IResourcesController.ResourcesKey loadKey)
		{
			this.resourcesController = resourcesController;
			this.loadKey = loadKey;
		}
		public void OnDestroy()
		{
			if(resourcesController == null) return;
			resourcesController.DestroyObject(loadKey, gameObject);

			resourcesController = null;
		}
	}
}
