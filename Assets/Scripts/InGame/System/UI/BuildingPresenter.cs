using System;
using InGame.Message;
using MessagePipe;
using UniRx;
using UnityEngine;

namespace InGame.System.UI
{
    public class BuildingPresenter : MonoBehaviour
    {
        [SerializeField] private BuildingView _view;

        private BuildingModel _model;
        private ISubscriber<AreaId, BuildingCountChangeMessage> _buildingCountChangeSubscriber;
        private IDisposable _subscription;

        public void InitializeAndShow(
            int playerCount,
            ISubscriber<AreaId, BuildingCountChangeMessage> buildingCountChangeSubscriber)
        {
            _model = new BuildingModel(playerCount, 10);
            _buildingCountChangeSubscriber = buildingCountChangeSubscriber;
            
            _view.Initialize(playerCount);
            _view.Show();
            Bind(playerCount);
        }

        public void Hide()
        {
            _view.HideAsync();
        }
        

        private void Bind(int playerCount)
        {
            for (var i = 0; i < playerCount; i++)
            {
                var areaId = new AreaId(i);

                // scene => model
                _buildingCountChangeSubscriber.Subscribe(areaId, message => _model.Decrease(areaId, message.CurrentCount)).AddTo(this);

                // model => view
                _model.BuildingCount(areaId).Subscribe(count => OnBuildingCountChanged(areaId, count)).AddTo(this);
            }
        }

        private void OnBuildingCountChanged(AreaId areaId, int currentCount)
        {
            _view.UpdateBuildingView(areaId, currentCount);
        }
        
        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}