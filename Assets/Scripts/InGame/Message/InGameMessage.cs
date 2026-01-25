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
    /// 接続復旧時のメッセージ
    /// </summary>
    public readonly struct ConnectionRecoverMessage
    {
        public readonly CharacterId PlayerId;
        public ConnectionRecoverMessage(CharacterId playerId)
        {
            PlayerId = playerId;
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

    /// <summary>
    /// Normalバトル終了メッセージ
    /// </summary>
    public readonly struct NormalBattleEndMessage
    {
    }

    /// <summary>
    /// ボス戦突入直後の初期化メッセージ
    /// </summary>
    public readonly struct InitBossBattleMessage : IStateParameter
    {
        public readonly int PlayerCount;
        public InitBossBattleMessage(int playerCount)
        {
            PlayerCount = playerCount;
        }
    }
    
    /// <summary>
    /// ボスバトル開始のメッセージ
    /// 初期化、演出が完全に終了した後に送信される
    /// </summary>
    public readonly struct BossBattleStartMessage
    {
    }

    /// <summary>
    /// ポーズマッチイベント開始メッセージ
    /// </summary>
    public readonly struct PoseMatchEventStartMessage
    {
    }

    /// <summary>
    /// ポーズマッチの結果を返すメッセージ
    /// </summary>
    public readonly struct PoseMatchEventResultMessage
    {
        public readonly bool IsSuccess;
        public PoseMatchEventResultMessage(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
    
    /// <summary>
    /// ポーズマッチイベント終了メッセージ
    /// </summary>
    public readonly struct PoseMatchEventEndMessage
    {
    }
    
    /// <summary>
    /// シールド展開メッセージ
    /// </summary>
    public readonly struct OpenShieldMessage
    {
    }
}