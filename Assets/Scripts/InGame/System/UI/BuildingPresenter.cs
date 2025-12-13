using System;
using Cysharp.Threading.Tasks;
using InGame.Message;
using MessagePipe;
using UniRx;
using UnityEngine;

namespace InGame.System.UI
{
    public class BuildingPresenter : MonoBehaviour, IPresenter
    {
        [SerializeField] private BuildingView _view;

        private BuildingModel _model;
        private ISubscriber<AreaId, BuildingCountChangeMessage> _buildingCountChangeSubscriber;
        private IDisposable _subscription;

        public void Initialize(
            int playerCount,
            ISubscriber<AreaId, BuildingCountChangeMessage> buildingCountChangeSubscriber)
        {
            _model = new BuildingModel(playerCount);
            _buildingCountChangeSubscriber = buildingCountChangeSubscriber;
            
            _view.Initialize(playerCount);
            Bind(playerCount);
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

        public void Dispose()
        {
            _subscription?.Dispose();
        }
        
        private void OnDestroy()
        {
            Dispose();
        }
    }
}