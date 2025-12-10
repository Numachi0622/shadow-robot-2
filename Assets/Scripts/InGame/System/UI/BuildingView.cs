using System.Collections.Generic;
using UnityEngine;

namespace InGame.System.UI
{
    public class BuildingView : MonoBehaviour
    {
        [SerializeField] private List<BuildingLineView> _buildingLines;
        
        private readonly Vector2[,] _displayPosition = new Vector2[,]
        {
            { new Vector2(0, -450), Vector2.zero, Vector2.zero },
            { new Vector2(-480f, -450f), new Vector2(480f, -450f), Vector2.zero },
            { new Vector2(-640f, -450f), new Vector2(0f, -450f), new Vector2(640f, -450f) },
        };

        public void Initialize(int playerCount)
        {
            var displayCount = Mathf.Min(playerCount, _buildingLines.Count);
            for (var i = 0; i < displayCount; i++)
            {
                var isActive = i < displayCount;
                var position = _displayPosition[playerCount - 1, i];
                _buildingLines[i].Initialize(isActive, position);
            }
        }
        
        public void UpdateBuildingView(AreaId areaId, int buildingCount)
        {
            _buildingLines[areaId.Value]?.UpdateBuildingLine(buildingCount);
        }
    }
}