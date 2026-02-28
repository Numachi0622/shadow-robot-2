using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace InGame.Character
{
    public class LongLaser : MonoBehaviour
    {
        [SerializeField] private Renderer _warningEffectRenderer;
        [SerializeField] private float _maxWarningFadeValue;
        [SerializeField] private float _minWarningFadeValue;
        [SerializeField] private float _initialFadeInterval = 0.5f;
        [SerializeField] private float _finalFadeInterval = 0.1f;
        [SerializeField] private ParticleSystem _chargeEffect;
        private Sequence _warningEffectSequence;
        private Material _warningEffectMaterial;
        private CancellationTokenSource _cts;
        
        public async UniTask PlayWarningEffectAsync(float duration)
        {
            if (_warningEffectMaterial == null)
            {
                _warningEffectMaterial = _warningEffectRenderer.material;
            }

            _cts = new CancellationTokenSource();
            _warningEffectRenderer.gameObject.SetActive(true);
            _chargeEffect.gameObject.SetActive(true);
            
            _warningEffectSequence?.Kill();
            _warningEffectSequence = DOTween.Sequence();

            float elapsed = 0f;

            // AttackReady終了時に急にエフェクトが消えると変なので、少し猶予を持たせてから終了させる
            while (elapsed < duration)
            {
                // 経過時間の割合に基づいてfadeIntervalを計算（0.0 -> 1.0）
                float progress = elapsed / duration;
                float currentFadeInterval = Mathf.Lerp(_initialFadeInterval, _finalFadeInterval, progress);

                _warningEffectSequence.Append(
                    _warningEffectMaterial.DOFade(_maxWarningFadeValue, currentFadeInterval)
                );
                _warningEffectSequence.Append(
                    _warningEffectMaterial.DOFade(_minWarningFadeValue, currentFadeInterval)
                );

                elapsed += currentFadeInterval * 2;
            }

            try
            {
                await _warningEffectSequence.Play().WithCancellation(_cts.Token);
            }
            finally
            {
                _warningEffectRenderer.gameObject.SetActive(false);
                _chargeEffect.gameObject.SetActive(false);
            }
        }

        public void StopWarningEffect()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}