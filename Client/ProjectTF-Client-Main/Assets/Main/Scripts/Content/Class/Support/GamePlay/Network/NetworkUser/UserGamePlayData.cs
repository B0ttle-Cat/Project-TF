using BC.ODCC;

using UnityEngine;
namespace TFContent
{
	public class UserGamePlayData : DataObject
	{
		public UserGamePlayData() : base()
		{

		}

		public int currentNodeIndex;
		public Vector2Int currentTableIndex;

		public int characterID;





		protected override void Disposing()
		{

		}
	}
}