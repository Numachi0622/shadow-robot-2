using System.Collections.Generic;
using UnityEngine;

namespace InGame.Environment
{
    public class MainStageManager : MonoBehaviour
    {
        [SerializeField] private List<MainStageElement> _elements;
        [SerializeField] private List<Transform> _playerSpawnPoints;
        [SerializeField] private Transform _buildingParent;
        private readonly List<Vector3> _buildingPositions = new();

        public IReadOnlyList<Vector3> BuildingPositions => _buildingPositions;
        public IReadOnlyList<Vector3> PlayerSpawnPositions => _playerSpawnPoints.ConvertAll(point => point.position);
        public Transform BuildingParent => _buildingParent;

        public void Initialize()
        {
            foreach (var element in _elements)
            {
                element.Initialize();
                foreach (var pos in element.BuildingPositions)
                {
                    _buildingPositions.Add(pos);
                }
            }
        }
    }
}