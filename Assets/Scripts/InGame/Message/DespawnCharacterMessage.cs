using InGame.Character;

namespace InGame.Message
{
    public struct DespawnCharacterMessage
    {
        public readonly CharacterId CharacterId;
        public DespawnCharacterMessage(CharacterId characterId)
        {
            CharacterId = characterId;
        }
    }
}