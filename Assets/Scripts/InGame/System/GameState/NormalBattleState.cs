using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace InGame.System
{
    public class NormalBattleState : StateMachine<InGameCore>.State
    {
        private CancellationTokenSource _cts;
        private IDisposable _subscription;

        private void GameStartPlayerInitialize(CharacterId characterId, int playerCount)
        {
            Owner.GameStartPlayerInitPublisher.Publish(characterId, new GameStartPlayerInitMessage(
                Owner.MainStageManager.PlayerSpawnPositions[characterId.Value],
                playerCount
            ));
        }

        private void SubscriptionBind(InGameContext context)
        {
            var bag = DisposableBag.CreateBuilder();
            
            // 接続復旧時のプレイヤー初期化購読も登録しておく
            Owner.ConnectionRecoverSubscriber.Subscribe(msg =>
            {
                GameStartPlayerInitialize(msg.PlayerId, context.PlayerCount);
            }).AddTo(bag);
            
            // Character生成リクエスト購読
            Owner.CharacterSpawnRequestSubscriber.Subscribe(request =>
            {
                var totalPlayerCount = Owner.Context.PlayerCount;
                
                // プレイヤー数を超えるCharacterIdの生成リクエストは無視する
                if (request.TryRequestProcess(totalPlayerCount))
                {
                    Owner.SpawnCharacterPublisher.Publish(request.SpawnCharacterMessage);
                }
            }).AddTo(bag);
            
            // Character消滅リクエスト購読
            Owner.CharacterDespawnRequestSubscriber.Subscribe(request =>
            {
                Owner.DespawnCharacterPublisher.Publish(request.DespawnCharacterMessage);
            }).AddTo(bag);
            
            _subscription = bag.Build();
        }
        
        public override async void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalBattleState] OnEnter");
            
            // プレイヤー数に応じてゲーム環境を構築する
            if (parameter is not InitGameMessage initGameMessage) return;
            var context = new InGameContext(initGameMessage.PlayerCount);
            Owner.SetContext(context);
            
            // プレイヤーをゲームが開始できる状態に初期化
            foreach (var chara in Owner.CharacterRegistry.GetAllPlayers())
            {
                var playerCore = chara as PlayerCore;
                if (playerCore == null) continue;

                var characterId = playerCore.PlayerId;
                GameStartPlayerInitialize(characterId, context.PlayerCount);
            }
            
            SubscriptionBind(context);
            
            Owner.StageReferences.MainStage.SetActive(true);
            
            // ビルの生成
            var areaCount = Mathf.Min(context.PlayerCount, Owner.MainStageManager.AreaCount);
            for (var i = 0; i < areaCount; i++)
            {
                var buildingPositions = Owner.MainStageManager.BuildingPositionsByArea(new AreaId(i));
                foreach (var pos in buildingPositions)
                {
                    var buildingPrefab = Owner.BuildingPrefabs[Random.Range(0, Owner.BuildingPrefabs.Count)];
                    Owner.CreateBuildingPublisher.Publish(new CreateBuildingMessage(
                        buildingPrefab,
                        new AreaId(i),
                        pos,
                        Quaternion.identity,
                        Owner.MainStageManager.BuildingParent
                    ));
                }
            }
            
            // ゲーム初期化完了通知
            var buildingCountPerArea = Owner.MainStageManager.BuildingCountPerArea;
            initGameMessage = new InitGameMessage(context.PlayerCount, buildingCountPerArea);
            Owner.InitGamePublisher.Publish(initGameMessage);

            _cts = new CancellationTokenSource();
            try
            {
                await NormalBattleLoopAsync(initGameMessage, _cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }
        
        public override void OnExit()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _subscription?.Dispose();
            
            Owner.NormalBattleEndPublisher.Publish(new NormalBattleEndMessage());
        }

        private async UniTask NormalBattleLoopAsync(InitGameMessage message, CancellationToken ct)
        {
            if (!GameConst.NoAnimationMode)
            {
                await Owner.InGameUIController.ShowAndHideBattleStartViewAsync();
            }

            while (true)
            {
                var areaCount = Mathf.Min(message.PlayerCount, Owner.MainStageManager.EnemySpawnPositions.Count);
                for (var i = 0; i < areaCount; i++)
                {
                    var areaId = new AreaId(i);
                    if (Owner.CharacterRegistry.IsEnemyFullByArea(areaId))
                    {
                        continue;
                    }
                        
                    var pos = Owner.MainStageManager.EnemySpawnPositions[i];
                    Owner.SpawnCharacterPublisher.Publish(new SpawnCharacterMessage(
                        CharacterType.NormalEnemy,
                        pos,
                        Quaternion.Euler(0, 180, 0), 
                        areaId,
                        TargetType.Building,
                        1
                    ));
                }

                await UniTask.Delay(TimeSpan.FromSeconds(GameConst.NormalEnemyGenerateInterval), cancellationToken: ct);
            }
        }
    }
}
