using System;

namespace InGame.Character
{
    [Serializable]
    public struct CharacterId
    {
        public readonly int Value;
        public CharacterId(int value)
        {
            Value = value;
        }
    }
}