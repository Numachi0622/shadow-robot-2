using InGame.Character;
using InGame.System;

namespace InGame.Message
{
    public readonly struct BuildingDestroyedMessage
    {
        public readonly AreaId AreaId;
        public readonly BuildingCore Building;

        public BuildingDestroyedMessage(AreaId areaId, BuildingCore building)
        {
            AreaId = areaId;
            Building = building;
        }
    }

    public readonly struct EnemyDestroyedMessage
    {
        public readonly AreaId AreaId;
        public readonly EnemyCore Enemy;

        public EnemyDestroyedMessage(AreaId areaId, EnemyCore enemy)
        {
            AreaId = areaId;
            Enemy = enemy;
        }
    }
    
    /// <summary>
    /// 敵を強制敵に消すメッセージ
    /// </summary>
    public readonly struct AllEnemyDespawnMessage
    {
    }

    /// <summary>
    /// プレイヤーを強制的に消すメッセージ
    /// </summary>
    public readonly struct AllPlayerDespawnMessage
    {
    }

    public readonly struct InitBossBattleMessage : IStateParameter
    {
        public readonly int PlayerCount;
        public InitBossBattleMessage(int playerCount)
        {
            PlayerCount = playerCount;
        }
    }
}