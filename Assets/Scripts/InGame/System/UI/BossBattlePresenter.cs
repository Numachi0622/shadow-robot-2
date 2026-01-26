using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.System.UI
{
    public enum WarningType
    {
        BossAppearance,
        FireBall,
        FireHoming
    }
    
    public class BossBattlePresenter : MonoBehaviour
    {
        [SerializeField] private BossBattleView _view;

        private readonly Dictionary<WarningType, string> _warningMessages = new()
        {
            { WarningType.BossAppearance, "ボスしゅつげん!" },
            { WarningType.FireBall, "ジャンプしてよけろ!" },
            { WarningType.FireHoming, "パンチしてはねかえせ!" }
        };

        public void Initialize()
        {
            _view.Initialize();
        }

        public async UniTask ShowWarningAsync(WarningType warningType, int fadeCount, float durationPerCount)
        {
            if (!_warningMessages.TryGetValue(warningType, out var message)) return;
            await _view.ShowWarningAsync(message, fadeCount, durationPerCount);
        }
        
        public async UniTask ShowAndHideMissionPopupAsync()
        {
            await _view.ShowAndHidePopupAsync(2f);
        } 
    }
}