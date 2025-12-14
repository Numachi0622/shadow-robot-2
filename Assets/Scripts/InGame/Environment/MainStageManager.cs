using System.Collections.Generic;
using InGame.System;
using NUnit.Framework;
using UnityEngine;

namespace InGame.Environment
{
    public class MainStageManager : MonoBehaviour
    {
        [SerializeField] private List<MainStageElement> _elements;
        [SerializeField] private List<Transform> _playerSpawnPoints;
        [SerializeField] private List<Transform> _enemySpawnPoints;
        [SerializeField] private Transform _buildingParent;
        [SerializeField] private Transform _bossSpawnPoint;
        private readonly Dictionary<AreaId, IReadOnlyList<Vector3>> _buildingPositions = new();

        public IReadOnlyList<Vector3> PlayerSpawnPositions => _playerSpawnPoints.ConvertAll(point => point.position);
        public IReadOnlyList<Vector3> EnemySpawnPositions => _enemySpawnPoints.ConvertAll(point => point.position);
        public Vector3 BossEnemySpawnPosition => _bossSpawnPoint.position;
        public Transform BuildingParent => _buildingParent;

        public void Initialize()
        {
            var areaIdValue = 0;
            foreach (var element in _elements)
            {
                element.Initialize();

                var positions = new List<Vector3>();
                foreach (var pos in element.BuildingPositions)
                {
                    positions.Add(pos);
                }
                _buildingPositions.Add(new AreaId(areaIdValue), positions);
                
                areaIdValue++;
            }
        }
        
        public IReadOnlyList<Vector3> BuildingPositionsByArea(AreaId areaId)
        {
            return _buildingPositions.GetValueOrDefault(areaId);
        }
        
        public int AreaCount => _elements.Count;
    }
}