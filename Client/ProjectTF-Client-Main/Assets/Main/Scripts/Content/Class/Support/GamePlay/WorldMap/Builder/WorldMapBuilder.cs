using BC.ODCC;
using BC.Sequence;

namespace TFContent
{
	public class WorldMapBuilder : ComponentBehaviour
	{
		SequencePlayer mapBuildPlayer;
		WorldMapBuildData worldMapBuildData;
		protected override void BaseAwake()
		{
			var mapBuildGraph = SequenceBuilder.Create("MapBuilder")
				.Sequence("MapBuilder")
					.Logger("Start - MapBuilder")
					.CallAction(WorldMapBuilder_GetBuilderData)
					.Condition(() => worldMapBuildData != null)
						.True(NodeBuilder.CallAction(null))
					.Logger("End - MapBuilder")
				.Return();
			mapBuildPlayer = SequenceBuilder.Build(mapBuildGraph);
		}

		protected override void BaseStart()
		{
			base.BaseStart();
		}

		private void WorldMapBuilder_GetBuilderData()
		{
			ThisContainer.TryGetData(out worldMapBuildData);
		}
	}
}
