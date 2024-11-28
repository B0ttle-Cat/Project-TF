using BC.ODCC;
namespace TFContent
{
	public class WorldMapBuildInfo : DataObject
	{
		public int[,] roomArray;


		public WorldMapBuildInfo() : base()
		{

		}

		protected override void Disposing()
		{
			roomArray = null;
		}
	}
}