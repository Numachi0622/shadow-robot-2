using System;
using Cysharp.Threading.Tasks;
using UniRx;
using InGame.Message;
using UnityEngine;

namespace InGame.System.UI
{
    public class NormalBattlePresenter : MonoBehaviour
    {
        [SerializeField] private NormalBattleView _view;
        private NormalBattleModel _model;
        private UniTaskCompletionSource _normalBattleCompletionSource;
        
        public void Initialize()
        {
            _view.Initialize();
            _normalBattleCompletionSource = new UniTaskCompletionSource();
        }
        
        public async UniTask ShowAndHideBattleStartViewAsync()
        {
            await _view.ShowAndHideBattleStartViewAsync();
        }

        public async UniTask ShowAndHideRemainingEnemiesCountViewAsync(int playerCount)
        {
            await _view.ShowAndHideRemainingEnemiesCountViewAsync(playerCount, _normalBattleCompletionSource);   
        }
        
        public void SetRemainingEnemiesCount(UpdateKillCountMessage message)
        {
            if (_model == null)
            {
                _model = new NormalBattleModel(message.TargetKillCount);
                _model.RemainingEnemiesCount.Subscribe(_view.UpdateRemainingEnemiesCount).AddTo(this);
            }
            
            _model.SetRemainingEnemiesCount(message);
        }
        
        public void EndNormalBattle()
        {
            _normalBattleCompletionSource?.TrySetResult();
        }
    }
}