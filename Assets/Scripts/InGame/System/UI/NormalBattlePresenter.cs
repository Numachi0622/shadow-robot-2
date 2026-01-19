using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.System.UI
{
    public class NormalBattlePresenter : MonoBehaviour
    {
        [SerializeField] private NormalBattleView _view;
        
        public void Initialize()
        {
            _view.Initialize();
        }
        
        public async UniTask ShowAndHideBattleStartViewAsync()
        {
            await _view.ShowAndHideBattleStartViewAsync();
        }
    }
}