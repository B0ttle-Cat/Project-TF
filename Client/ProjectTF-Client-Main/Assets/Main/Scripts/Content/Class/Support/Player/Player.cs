using UnityEngine;

using BC.ODCC;

namespace TFContent.Player
{
    public class Player : ObjectBehaviour
    {

		public void SetPlayerData(PlayerData playerData)
		{
			if (ThisContainer.TryGetData<PlayerData>(out var data))
			{
				data.SetData(playerData);
			}
			else
			{
				ThisContainer.AddData(playerData);
			}
		}
	}
}
