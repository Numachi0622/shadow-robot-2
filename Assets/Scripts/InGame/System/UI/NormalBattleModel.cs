using InGame.Message;
using UniRx;

namespace InGame.System.UI
{
    public class NormalBattleModel
    {
        private readonly ReactiveProperty<int> _remainingEnemiesCount;
        public IReadOnlyReactiveProperty<int> RemainingEnemiesCount => _remainingEnemiesCount;
        
        public NormalBattleModel(int targetCount)
        {
            _remainingEnemiesCount = new ReactiveProperty<int>(targetCount);
        }
        
        public void SetRemainingEnemiesCount(UpdateKillCountMessage message)
        {
            var remainingCount = message.TargetKillCount - message.CurrentKillCount;
            _remainingEnemiesCount.Value = remainingCount;
        }
    }
}