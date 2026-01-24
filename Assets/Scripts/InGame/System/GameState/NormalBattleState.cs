using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace InGame.System
{
    public class NormalBattleState : StateMachine<InGameCore>.State
    {
        private CancellationTokenSource _cts;
        
        public override async void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalBattleState] OnEnter");
            
            // プレイヤー数に応じてゲーム環境を構築する
            if (parameter is not InitGameMessage initGameMessage) return;
            
            Owner.InitGamePublisher.Publish(initGameMessage);
            
            foreach (var chara in Owner.CharacterRegistry.GetAllPlayers())
            {
                var playerCore = chara as PlayerCore;
                if (playerCore == null) continue;

                var characterId = playerCore.PlayerId;
                Owner.GameStartPlayerInitPublisher.Publish(characterId, new GameStartPlayerInitMessage(
                    Owner.MainStageManager.PlayerSpawnPositions[characterId.Value],
                    initGameMessage.PlayerCount
                ));
            }
            
            Owner.StageReferences.MainStage.SetActive(true);
            
            // ビルの生成
            var areaCount = Mathf.Min(initGameMessage.PlayerCount, Owner.MainStageManager.AreaCount);
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
