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
        public readonly TargetType TargetType;

        public SpawnCharacterMessage(CharacterType characterType, Vector3 position, Quaternion rotation, AreaId areaId, TargetType targetType, int totalPlayerCount = -1)
        {
            CharacterId = new CharacterId(-1);
            CharacterType = characterType;
            Position = position;
            Rotation = rotation;
            AreaId = areaId;
            TotalPlayerCount = totalPlayerCount;
            TargetType = targetType;
        }
        
        public SpawnCharacterMessage(CharacterId characterId, CharacterType characterType, Vector3 position, Quaternion rotation)
        {
            CharacterId = characterId;
            CharacterType = characterType;
            Position = position;
            Rotation = rotation;
            AreaId = new AreaId(-1);
            TotalPlayerCount = -1;
            TargetType = TargetType.None;
        }
        
        public SpawnCharacterMessage(CharacterId characterId, CharacterType characterType, Vector3 position, Quaternion rotation, int totalPlayerCount)
        {
            CharacterId = characterId;
            CharacterType = characterType;
            Position = position;
            Rotation = rotation;
            AreaId = new AreaId(-1);
            TotalPlayerCount = totalPlayerCount;
            TargetType = TargetType.None;
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
    
    public enum TargetType
    {
        None,
        Player,
        Enemy,
        Building
    }
}
