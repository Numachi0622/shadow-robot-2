using UnityEngine;

namespace InGame.System.UI
{
    public class GameOverPresenter : MonoBehaviour
    {
        [SerializeField] private GameOverView _view;
        
        public void Initialize()
        {
            _view.Initialize();
        }
        
        public void ShowGameOverView()
        {
            _view.ShowGameOverView();
        }
    }
}