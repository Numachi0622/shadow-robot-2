using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.Character;
using InGame.Event;
using InGame.Message;
using InGame.System.UI;
using UnityEngine;
using Utility;
using MessagePipe;

namespace InGame.System
{
    public class BossBattleState : StateMachine<InGameCore>.State
    {
        private CancellationTokenSource _cts;
        private IDisposable _subscription;
        private PoseDataService _poseDataService;
        
        public override async void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[BossBattleState] OnEnter");

            var context = Owner.Context;
            if (context == null)
            {
                Debug.LogError("[BossBattleState] Context is null");
                return;
            }
            
            // PoseDataServiceの初期化とプリロード
            _poseDataService = new PoseDataService();
            await _poseDataService.PreloadAsync();

            var bag = DisposableBag.CreateBuilder();
            Owner.PoseMatchEventStartSubscriber.Subscribe(_ => PoseMatchEventAsync().Forget()).AddTo(bag);

            _cts = new CancellationTokenSource();
            try
            {
                await BossBattleEnterAsync(context.PlayerCount, _cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask BossBattleEnterAsync(int playerCount, CancellationToken ct)
        {
            // 雑魚敵は全て削除しておく
            Owner.AllEnemyDespawnMessage.Publish(new AllEnemyDespawnMessage());

            if (!GameConst.NoAnimationMode)
            {
                // ボス出現警告表示
                await Owner.InGameUIController.ShowWarningAsync(WarningType.BossAppearance);   
            }
            
            // ビルを非表示
            var buildings = Owner.CharacterRegistry.GetAllBuildings();
            foreach (var building in buildings)
            {
                building.gameObject.SetActive(false);
            }
            
            // ボスの生成
            Owner.SpawnCharacterPublisher.Publish(new SpawnCharacterMessage(
                Message.CharacterType.BossEnemy,
                Owner.MainStageManager.BossEnemySpawnPosition,
                Quaternion.Euler(0, 180, 0),
                new AreaId(-1),
                TargetType.Player
            ));
            
            // 敵出現カットシーン
            var bossEnterSceneContext = new BossEnterCutSceneContext()
            {
                Director = Owner.PlayableDirectorReferences.BossBattleStartCutSceneDirector,
                BuildingCount = buildings.Count
            };
            await CutSceneManager.LoadAndPlayAsync<BossEnterCutScene>(bossEnterSceneContext, ct);
            
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
            else
            {
                Owner.GameStartPlayerInitPublisher.Publish(
                    new CharacterId(0), 
                    new GameStartPlayerInitMessage(Vector3.zero, 1)
                );
            }
            
            if (playerCount > 1 && playerCount <= GameConst.MaxPlayerCount)
            {
                // 合体カットシーン
                var combineSceneContext = new CombineCutSceneContext()
                {
                    Director = Owner.PlayableDirectorReferences.CombineCutSceneDirector
                };
                await CutSceneManager.LoadAndPlayAsync<CombineCutScene>(combineSceneContext, ct);

                Owner.CombineCompletePublisher.Publish(new CombineCompleteMessage());
                
                // 部位分担説明UI表示
                await Owner.InGameUIController.ShowAndHidePartsDescriptionAsync(playerCount);
            }
            
            // MissionPopup表示
            await Owner.InGameUIController.ShowAndHideMissionPopupAsync();
            
            // 演出が全て終了した後にバトル開始メッセージを送信
            Owner.BossBattleStartPublisher.Publish(new BossBattleStartMessage());
        }

        private async UniTask PoseMatchEventAsync()
        {
            var player = Owner.CharacterRegistry.GetAllPlayers().FirstOrDefault() as PlayerCore;
            if (player == null) return;

            // ランダムにPoseDataを選択
            var poseData = await _poseDataService.GetRandomPoseAsync();
            if (poseData == null)
            {
                Debug.LogError("[BossBattleState] Failed to get PoseData");
                return;
            }

            // カメラをポーズマッチ用の位置に移動
            var camera = player.PlayerCamera;
            var defaultPos = camera.transform.localPosition;
            var defaultRot = camera.transform.localRotation;
            var targetPos = new Vector3(0, 1, 0);
            camera.transform.SetParent(null);
            var cameraTask = DOTween.Sequence()
                .Append(camera.transform.DOMove(targetPos, 0.5f))
                .Join(camera.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.5f))
                .ToUniTask();
            
            // HPゲージを非表示
            var hpHideTask = Owner.InGameUIController.HideHitPointViewAsync();
            
            await UniTask.WhenAll(cameraTask, hpHideTask);

            // ポーズマッチUI表示（PoseDataを渡す）
            await Owner.InGameUIController.ShowPoseMatchViewAsync(poseData);

            // ポーズマッチの結果が返ってくるまで待機
            var resultSource = new UniTaskCompletionSource<PoseMatchEventResultMessage>();
            var bag = DisposableBag.CreateBuilder();
            Owner.PoseMatchEventResultSubscriber.Subscribe(msg => resultSource.TrySetResult(msg)).AddTo(bag);
            _subscription = bag.Build();

            var result = await resultSource.Task;
            if (result.IsSuccess)
            {
                // ポーズマッチ成功演出
                await Owner.InGameUIController.PoseMatchSuccessAnimationAsync();
                
                // シールド展開メッセージ発行
                Owner.OpenShieldPublisher.Publish(new OpenShieldMessage());
            }
            
            // ポーズマッチUI非表示
            await Owner.InGameUIController.HidePoseMatchViewAsync();
            
            // カメラを元の位置に戻す
            camera.transform.SetParent(player.transform);
            cameraTask = DOTween.Sequence()
                .Append(camera.transform.DOLocalMove(defaultPos, 0.5f))
                .Join(camera.transform.DOLocalRotateQuaternion(defaultRot, 0.5f))
                .ToUniTask();
            
            // HPゲージを再表示
            var hpShowTask = Owner.InGameUIController.ShowHitPointViewAsync();
            
            await UniTask.WhenAll(cameraTask, hpShowTask);

            Owner.PoseMatchEventEndPublisher.Publish(new PoseMatchEventEndMessage());
        }
        
        public override void OnExit()
        {
            _cts?.Cancel();
            _cts = null;
            _subscription?.Dispose();
            _subscription = null;
            _poseDataService?.Dispose();
            _poseDataService = null;
            
            Owner.InGameUIController.HideHitPointViewAsync().Forget();
        }
    }
}
