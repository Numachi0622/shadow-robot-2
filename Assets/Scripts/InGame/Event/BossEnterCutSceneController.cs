using System.Collections.Generic;
using DG.Tweening;
using InGame.Character;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;

namespace InGame.Event
{
    public class BossEnterCutSceneController : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _director;
        
        [Header("ビルの分離アニメーションパラメータ")]
        [SerializeField] private Transform _buildingTransform;
        [SerializeField] private float _leaveHeight = 5f;
        [SerializeField] private float _leaveInterval = 0.5f;
        [SerializeField] private float _leaveDuration = 0.5f;
        [SerializeField] private int _shakeVibrato = 15;
        [SerializeField] private ParticleSystem _dustEffect;

        [Header("ビルの吸収アニメーションパラメータ")] 
        [SerializeField] private int _buildingCount = 10;
        [SerializeField] private float _scale = 0.04f;
        [SerializeField] private float _absorptionDuration = 0.3f;
        [SerializeField] private CharacterPrefabs _prefabs;
        [SerializeField] private Transform _portalTransform;
        
        [Header("フェードアニメーションパラメータ")]
        [SerializeField] private CanvasGroup _fadeCanvasGroup;
        
        private Sequence _leaveBuildingSequence;
        private Sequence _absorptionSequence;
        private Sequence _fadeSequence;
        
        private const int MaxBuildingCount = 10;
        private const int BossEnterCutSceneLayer = 13;

        public void Initialize(int buildingCount)
        {
            _buildingCount = buildingCount;
        }

        [Button]
        public void PlayLeaveBuildingSequence()
        {
            _leaveBuildingSequence?.Kill();

            _leaveBuildingSequence = DOTween.Sequence()
                .SetLink(gameObject)
                .AppendCallback(() => _dustEffect.Play())
                .Append(_buildingTransform.DOShakePosition(1f, vibrato:_shakeVibrato, fadeOut:false))
                .AppendInterval(_leaveInterval)
                .Append(_buildingTransform.DOLocalMoveY(_leaveHeight, _leaveDuration).SetEase(Ease.OutQuint));
        }

        [Button]
        public void PlayAbsorptionSequence()
        {
            _absorptionSequence?.Kill();
            
            // 入力分のビルを生成
            var count = Mathf.Clamp(_buildingCount, 1, MaxBuildingCount);
            var buildingList = new List<Transform>(count);
            for (int i = 0; i < count; i++)
            {
                var pos = new Vector3(
                    Random.Range(-5f, 5f), 
                    Random.Range(1f, 4f),
                    0f
                );

                var dir = (_portalTransform.position - pos).normalized;
                var rot = Quaternion.LookRotation(transform.forward, dir);
                var building = Instantiate(_prefabs.BuildingPrefabs[Random.Range(0, _prefabs.BuildingPrefabs.Count)], pos, rot, transform);
                building.transform.localScale = Vector3.one * _scale;
                building.gameObject.layer = BossEnterCutSceneLayer;
                building.transform.GetChild(0).gameObject.layer = BossEnterCutSceneLayer;
                
                buildingList.Add(building.transform);
            }

            _absorptionSequence = DOTween.Sequence()
                .SetLink(gameObject);

            foreach (var b in buildingList)
            {
                _absorptionSequence.Append(b.DOMove(_portalTransform.position, _absorptionDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => b.gameObject.SetActive(false)));
            }

            _absorptionSequence.AppendInterval(0.5f);
            _absorptionSequence.AppendCallback(FadeInSequence);
        }
        
        public void FadeOutSequence()
        {
            _fadeSequence?.Kill();
            _fadeCanvasGroup.alpha = 1f;

            _fadeSequence = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(_fadeCanvasGroup.DOFade(0f, 0.5f));
        }

        public void FadeInSequence()
        {
            _fadeSequence?.Kill();
            _fadeCanvasGroup.alpha = 0f;

            _fadeSequence = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(_fadeCanvasGroup.DOFade(1f, 0.5f))
                .AppendCallback(() => _director.Stop());
        }
    }
}
