using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace TFContent.Playspace
{
	public class WorldMapBuilder : ComponentBehaviour
	{
		[ShowInInspector, ReadOnly]
		private WorldMapUserSettingData worldMapBuildData;

		[SerializeField]
		private RoomObject roomObject;
		protected override void BaseAwake()
		{

		}

		protected override void BaseStart()
		{
			ThisContainer.TryGetData<WorldMapUserSettingData>(out worldMapBuildData);
		}
	}
}
