using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.System.UI
{
    public class PoseMatchPresenter : MonoBehaviour
    {
        [SerializeField] private PoseMatchView _view;
        [SerializeField] private float _limitTime = 10f;
        private PoseMatchModel _model;

        public async UniTask ShowAsync()
        {
            await _view.ShowAsync();
        }
        
        public async UniTask HideAsync()
        {
            await _view.HideAsync();
        }
        
        public async UniTask PoseMatchSuccessAnimationAsync()
        {
            await _view.PoseMatchSuccessAnimationAsync();
        }
    }
}