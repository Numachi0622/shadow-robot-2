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

    public enum DescriptionType
    {
        LeftHand,
        RightHand,
        Foot,
        LeftHandAndRightHand
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

        private readonly Dictionary<DescriptionType, string> _descriptionTitles = new()
        {
            { DescriptionType.LeftHand, "ひだりて" },
            { DescriptionType.RightHand, "みぎて" },
            { DescriptionType.Foot, "りょうあし" },
            { DescriptionType.LeftHandAndRightHand, "りょうて" }
        };
        
        private readonly Dictionary<DescriptionType, string> _descriptionMessages = new()
        {
            { DescriptionType.LeftHand, "パンチでこうげきをはねかえそう！"},
            { DescriptionType.RightHand, "パンチでこうげきをはねかえそう！" },
            { DescriptionType.Foot, "ジャンプしてこうげきをよけよう！" },
            { DescriptionType.LeftHandAndRightHand, "パンチでこうげきをはねかえそう！" }
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
        
        public async UniTask ShowAndHidePartsDescriptionAsync(int playerCount)
        {
            var descriptions = new List<(string title, string message)>();
            if (playerCount == 2)
            {
                descriptions.Add((_descriptionTitles[DescriptionType.LeftHandAndRightHand], _descriptionMessages[DescriptionType.LeftHandAndRightHand]));
                descriptions.Add((_descriptionTitles[DescriptionType.Foot], _descriptionMessages[DescriptionType.Foot]));
            }
            else if (playerCount == 3)
            {
                descriptions.Add((_descriptionTitles[DescriptionType.LeftHand], _descriptionMessages[DescriptionType.LeftHand]));
                descriptions.Add((_descriptionTitles[DescriptionType.RightHand], _descriptionMessages[DescriptionType.RightHand]));
                descriptions.Add((_descriptionTitles[DescriptionType.Foot], _descriptionMessages[DescriptionType.Foot]));
            }
            
            await _view.ShowAndHidePartsDescriptionAsync(descriptions);
        }
    }
}