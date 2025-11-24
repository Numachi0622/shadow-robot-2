using InGame.Character;
using UnityEngine;

namespace InGame.Message
{
    /// <summary>
    /// キャラクター生成リクエスト
    /// </summary>
    public readonly struct SpawnCharacterMessage
    {
        public readonly CharacterId CharacterId;
        public readonly CharacterType CharacterType;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public SpawnCharacterMessage(CharacterType characterType, Vector3 position, Quaternion rotation)
        {
            CharacterId = new CharacterId(-1);
            CharacterType = characterType;
            Position = position;
            Rotation = rotation;
        }
        
        public SpawnCharacterMessage(CharacterId characterId, CharacterType characterType, Vector3 position, Quaternion rotation)
        {
            CharacterId = characterId;
            CharacterType = characterType;
            Position = position;
            Rotation = rotation;
        }
    }

    /// <summary>
    /// キャラクタータイプ
    /// </summary>
    public enum CharacterType
    {
        Player,
        NormalEnemy,
        BossEnemy,
        Building
    }
}
