using UnityEngine;

using BC.ODCC;

namespace TF.Content.Character
{
    public class CharacterData : DataObject
    {
        public int idx;
        public eCharacterType type;

        public void SetData(CharacterData data)
        {
            this.idx = data.idx;
            this.type = data.type;
		}
    }
}
