using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace InGame.System.UI
{
    public class BuildingModel
    {
        private readonly Dictionary<AreaId, ReactiveProperty<int>> _buildingCount = new();
        private readonly Subject<AreaId> _onAllDestroyedPerArea = new();

        public IReadOnlyReactiveProperty<int> BuildingCount(AreaId areaId) => _buildingCount.GetValueOrDefault(areaId);
        public IObservable<AreaId> OnAllDestroyedPerArea => _onAllDestroyedPerArea;

        public BuildingModel(int areaCount, int buildingCount = 7)
        {
            for (var i = 0; i < areaCount; i++)
            {
                _buildingCount[new AreaId(i)] = new ReactiveProperty<int>(buildingCount);
            }
        }

        public void Decrease(AreaId areaId, int decreasedValue)
        {
            _buildingCount[areaId].Value = Mathf.Max(decreasedValue, 0);
            if (_buildingCount[areaId].Value <= 0)
            {
                _onAllDestroyedPerArea.OnNext(areaId);
            }
        }
    }
}