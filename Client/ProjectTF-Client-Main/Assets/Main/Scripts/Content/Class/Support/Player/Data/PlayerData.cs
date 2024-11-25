using UnityEngine;

using BC.ODCC;

namespace TFContent.Player
{
    public class PlayerData : DataObject
    {
        private int playerIdx;

        public int PlayerIdx => playerIdx;

        public void SetData(PlayerData data)
        {

        }

		public PlayerData(int playerIdx)
        {
            this.playerIdx = playerIdx;
		}
	}
}
