using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using UnityEngine;
using Utility;

namespace InGame.System
{
    public class BossBattleState : StateMachine<InGameCore>.State
    {
        private CancellationTokenSource _cts;
        
        public override async void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[BossBattleState] OnEnter");

            if (parameter is not InitBossBattleMessage message) return;

            _cts = new CancellationTokenSource();
            try
            {
                await BossBattleLoopAsync(message.PlayerCount, _cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask BossBattleLoopAsync(int playerCount, CancellationToken ct)
        {
            // await todo: ボス出現警告UI
            
            Owner.SpawnCharacterPublisher.Publish(new SpawnCharacterMessage(
                Message.CharacterType.BossEnemy,
                Owner.MainStageManager.BossEnemySpawnPosition,
                Quaternion.Euler(0, 180, 0),
                new AreaId(-1)
            ));
            
            // await todo: 敵出現タイムライン

            if (playerCount > 1)
            {
                Owner.AllPlayerDespawnMessage.Publish(new AllPlayerDespawnMessage());
                Owner.SpawnCharacterPublisher.Publish(new SpawnCharacterMessage(
                    GameConst.BossPlayerId,
                    Message.CharacterType.CombinePlayer,
                    Vector3.zero,
                    Quaternion.identity,
                    playerCount
                ));   
            }
            
            var character = Owner.CharacterRegistry.GetAllPlayers().FirstOrDefault() as PlayerCore;
            if (character == null)
            {
                Debug.Log(Owner.CharacterRegistry.GetAllPlayers().Count);
                Debug.LogError("Player is Null");
                return;
            }

            // await todo: 合体演出
            
            // 演出が全て終了した後にバトル開始メッセージを送信
            Owner.BossBattleStartPublisher.Publish(new BossBattleStartMessage());
        }
    }
}
