using BC.ODCC;
namespace TFContent.Playspace
{
	public class WorldMapBuildInfo : DataObject
	{
		public WorldMapRawData worldMapRawData;

		public WorldMapBuildInfo() : base()
		{

		}

		protected override void Disposing()
		{
			worldMapRawData.Dispose();
		}
	}
}