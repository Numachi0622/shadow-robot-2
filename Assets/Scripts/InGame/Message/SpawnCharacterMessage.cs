using UnityEngine;

namespace InGame.Message
{
    /// <summary>
    /// キャラクター生成リクエスト
    /// </summary>
    public readonly struct SpawnCharacterMessage
    {
        public readonly CharacterType CharacterType;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public SpawnCharacterMessage(CharacterType characterType, Vector3 position, Quaternion rotation)
        {
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
