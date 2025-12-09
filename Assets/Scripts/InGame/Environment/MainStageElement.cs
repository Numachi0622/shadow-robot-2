using System.Collections.Generic;
using UnityEngine;

namespace InGame.Environment
{
    public class MainStageElement : MonoBehaviour
    {
        [SerializeField] private Transform _buildingParent;
        private readonly Queue<Vector3> _buildingPositions = new();

        public IReadOnlyCollection<Vector3> BuildingPositions => _buildingPositions;

        public void Initialize()
        {
            foreach (Transform child in _buildingParent)
            {
                _buildingPositions.Enqueue(child.position);
            }
        }
    }
}