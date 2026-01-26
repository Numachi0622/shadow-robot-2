using InGame.Character;
using InGame.System;
using InGame.System.UI;

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
    /// 雑魚敵のキル数更新メッセージ
    /// </summary>
    public readonly struct UpdateKillCountMessage
    {
        public readonly int CurrentKillCount;
        public readonly int TargetKillCount;
        public UpdateKillCountMessage(int current, int target)
        {
            CurrentKillCount = current;
            TargetKillCount = target;
        }
    }
    
    /// <summary>
    /// 敵を強制敵に消すメッセージ
    /// </summary>
    public readonly struct AllEnemyDespawnMessage
    {
    }

    /// <summary>
    /// 敵を強制的に停止させるメッセージ
    /// </summary>
    public readonly struct AllEnemyStopMessage
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
    /// 合体成功メッセージ
    /// </summary>
    public readonly struct CombineCompleteMessage
    {
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
    
    /// <summary>
    /// 警告表示メッセージ
    /// </summary>
    public readonly struct ShowWarningMessage
    {
        public readonly WarningType WarningType;
        public readonly int FadeCount;
        public readonly float FadeDuration;
        public ShowWarningMessage(WarningType warningType, int fadeCount, float fadeDuration)
        {
            WarningType = warningType;
            FadeCount = fadeCount;
            FadeDuration = fadeDuration;
        }
    }

    /// <summary>
    /// リザルトステートへ渡すコンテキストメッセージ
    /// </summary>
    public readonly struct ResultContextMessage : IStateParameter
    {
        public readonly int TotalBuildingCount;
        public ResultContextMessage(int totalBuildingCount)
        {
            TotalBuildingCount = totalBuildingCount;
        }
    }
}