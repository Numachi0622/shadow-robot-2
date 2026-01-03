using InGame.Character;
using InGame.System;
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
        public readonly AreaId AreaId;
        public readonly int TotalPlayerCount;

        public SpawnCharacterMessage(CharacterType characterType, Vector3 position, Quaternion rotation, AreaId areaId)
        {
            CharacterId = new CharacterId(-1);
            CharacterType = characterType;
            Position = position;
            Rotation = rotation;
            AreaId = areaId;
            TotalPlayerCount = -1;
        }
        
        public SpawnCharacterMessage(CharacterId characterId, CharacterType characterType, Vector3 position, Quaternion rotation)
        {
            CharacterId = characterId;
            CharacterType = characterType;
            Position = position;
            Rotation = rotation;
            AreaId = new AreaId(-1);
            TotalPlayerCount = -1;
        }
        
        public SpawnCharacterMessage(CharacterId characterId, CharacterType characterType, Vector3 position, Quaternion rotation, int totalPlayerCount)
        {
            CharacterId = characterId;
            CharacterType = characterType;
            Position = position;
            Rotation = rotation;
            AreaId = new AreaId(-1);
            TotalPlayerCount = totalPlayerCount;
        }
    }

    /// <summary>
    /// キャラクタータイプ
    /// </summary>
    public enum CharacterType
    {
        Player,
        CombinePlayer,
        NormalEnemy,
        BossEnemy,
        Building
    }
}
