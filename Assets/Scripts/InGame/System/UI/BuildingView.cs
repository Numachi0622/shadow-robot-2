using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using NotImplementedException = System.NotImplementedException;

namespace InGame.System.UI
{
    public class BuildingView : MonoBehaviour, IVisibilityController
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

        public bool IsActive => gameObject.activeSelf;
        public void Show()
        {
            if (gameObject.activeSelf) return;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (!gameObject.activeSelf) return;
            gameObject.SetActive(false);
        }

        public UniTask ShowAsync()
        {
            Show();
            return UniTask.CompletedTask;
        }

        public UniTask HideAsync()
        {
            Hide();
            return UniTask.CompletedTask;
        }
    }
}