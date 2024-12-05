using UnityEngine;

using BC.ODCC;

namespace TFContent.Character
{
	public interface ICharacterItemBox
	{
		void MoveItem(int itemId);
	}

	public class ItemBoxMove : ComponentBehaviour
	{

        public void Move()
        {

        }
    }
}