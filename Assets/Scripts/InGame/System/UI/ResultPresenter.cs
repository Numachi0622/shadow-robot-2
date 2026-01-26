using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.System.UI
{
    public class ResultPresenter : MonoBehaviour
    {
        [SerializeField] private ResultView _view;

        public void Initialize()
        {
            _view.Initialize();
        }
        
        public async UniTask PlayResultSequenceAsync(int buildingCount)
        {
            await _view.ResultViewAsync(buildingCount);
        }
    }
}