using System.Collections.Generic;
using UnityEngine;

namespace InGame.System.UI
{
    public class BuildingView : MonoBehaviour
    {
        [SerializeField] private List<BuildingLineView> _buildingLines;
        
        public void UpdateBuildingView(AreaId areaId, int buildingCount)
        {
            _buildingLines[areaId.Value]?.UpdateBuildingLine(buildingCount);
        }
    }
}