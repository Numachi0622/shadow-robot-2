using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.Character;
using InGame.Message;
using UnityEngine;
using Utility;
using MessagePipe;

namespace InGame.System
{
    public class BossBattleState : StateMachine<InGameCore>.State
    {
        private CancellationTokenSource _cts;
        private IDisposable _subscription;
        
        public override async void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[BossBattleState] OnEnter");

            if (parameter is not InitBossBattleMessage message) return;
            
            var bag = DisposableBag.CreateBuilder();
            Owner.PoseMatchEventStartSubscriber.Subscribe(_ => PoseMatchEventAsync().Forget()).AddTo(bag);

            _cts = new CancellationTokenSource();
            try
            {
                await BossBattleEnterAsync(message.PlayerCount, _cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask BossBattleEnterAsync(int playerCount, CancellationToken ct)
        {
            // await todo: ボス出現警告UI
            
            Owner.SpawnCharacterPublisher.Publish(new SpawnCharacterMessage(
                Message.CharacterType.BossEnemy,
                Owner.MainStageManager.BossEnemySpawnPosition,
                Quaternion.Euler(0, 180, 0),
                new AreaId(-1),
                TargetType.Player
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

        private async UniTask PoseMatchEventAsync()
        {
            var player = Owner.CharacterRegistry.GetAllPlayers().FirstOrDefault() as PlayerCore;
            if (player == null) return;

            var camera = player.PlayerCamera;
            var defaultPos = camera.transform.localPosition;
            var targetPos = new Vector3(0, 1, 0);
            camera.transform.SetParent(null);
            await camera.transform.DOMove(targetPos, 0.5f).ToUniTask();

            // ポーズマッチの結果が返ってくるまで待機
            var resultSource = new UniTaskCompletionSource<PoseMatchEventResultMessage>();
            var bag = DisposableBag.CreateBuilder();
            Owner.PoseMatchEventResultSubscriber.Subscribe(msg => resultSource.TrySetResult(msg)).AddTo(bag);
            _subscription = bag.Build();

            var result = await resultSource.Task;
            if (result.IsSuccess)
            {
                // todo: ポーズマッチ成功演出
                // todo: ガード展開
            }

            camera.transform.SetParent(player.transform);
            await camera.transform.DOLocalMove(defaultPos, 0.5f).ToUniTask();

            Owner.PoseMatchEventEndPublisher.Publish(new PoseMatchEventEndMessage());
        }
        
        public override void OnExit()
        {
            _cts?.Cancel();
            _cts = null;
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}
