using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace InGame.System.UI
{
    public class BuildingLineView : MonoBehaviour
    {
        [SerializeField] private List<CanvasGroup> _buildingElements;
        private Sequence _fadeSequence;

        public void Initialize(bool isActive, Vector2 position)
        {
            gameObject.SetActive(isActive);
            transform.localPosition = position;
        }

        public void UpdateBuildingLine(int buildingCount)
        {
            for (var i = _buildingElements.Count - 1; i >= buildingCount; i--)
            {
                var element = _buildingElements[i];
                if (element == null || element.alpha <= 0f) continue;
                
                _fadeSequence?.Kill();
                _fadeSequence = DOTween.Sequence()
                    .Append(element.DOFade(0f, 0.3f));
            }
        }
    }
}