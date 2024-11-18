using Sirenix.OdinInspector;

using UnityEngine;

namespace TF.System
{
	public class ResourcesState : MonoBehaviour
	{
		private IResourcesController resourcesController;
		[ReadOnly]
		private string assetPath;

		public void Init(IResourcesController resourcesController, string assetPath)
		{
			this.resourcesController = resourcesController;
			this.assetPath = assetPath;
		}
		public void OnDestroy()
		{
			if(resourcesController == null) return;
			resourcesController.DestroyObject(assetPath, gameObject);

			resourcesController = null;
		}
	}
}
