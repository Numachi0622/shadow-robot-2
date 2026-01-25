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
        private int _playerCount;
        private CancellationTokenSource _cts;
        private IDisposable _subscription;

        private void GameStartPlayerInitialize(CharacterId characterId, int playerCount)
        {
            Owner.GameStartPlayerInitPublisher.Publish(characterId, new GameStartPlayerInitMessage(
                Owner.MainStageManager.PlayerSpawnPositions[characterId.Value],
                playerCount
            ));
        }
        
        public override async void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalBattleState] OnEnter");
            
            // プレイヤー数に応じてゲーム環境を構築する
            if (parameter is not InitGameMessage initGameMessage) return;
            _playerCount = initGameMessage.PlayerCount;
            
            // プレイヤーをゲームが開始できる状態に初期化
            foreach (var chara in Owner.CharacterRegistry.GetAllPlayers())
            {
                var playerCore = chara as PlayerCore;
                if (playerCore == null) continue;

                var characterId = playerCore.PlayerId;
                GameStartPlayerInitialize(characterId, _playerCount);
            }
            
            // 接続復旧時のプレイヤー初期化購読も登録しておく
            var bag = DisposableBag.CreateBuilder();
            Owner.ConnectionRecoverSubscriber.Subscribe(msg =>
            {
                GameStartPlayerInitialize(msg.PlayerId, _playerCount);
            }).AddTo(bag);
            _subscription = bag.Build();
            
            Owner.StageReferences.MainStage.SetActive(true);
            
            // ビルの生成
            var areaCount = Mathf.Min(_playerCount, Owner.MainStageManager.AreaCount);
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
            initGameMessage = new InitGameMessage(_playerCount, buildingCountPerArea);
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
            
            Owner.AllEnemyDespawnMessage.Publish(new AllEnemyDespawnMessage());
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
                        Quaternion.identity, 
                        areaId,
                        TargetType.Building
                    ));
                }

                await UniTask.Delay(TimeSpan.FromSeconds(GameConst.NormalEnemyGenerateInterval), cancellationToken: ct);
            }
        }
    }
}
