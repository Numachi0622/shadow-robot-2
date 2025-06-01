using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EnemyEffect : MonoBehaviour
{
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private float _attackReadyShakeStrength = 0.1f;
    [SerializeField] private int _attackReadyShakeVibrato = 20;
    [SerializeField] private float _attackReadyShakeDuration = 0.5f;

    private Material _bodyMaterial;
    private Vector3 _shakeOffset;
    
    private Sequence _blinkSequence;
    private Sequence _shakeSequence;
    private Sequence _disovleSequence;
    
    public void Initialize()
    {
        _bodyMaterial = _skinnedMeshRenderer.material;
    }

    private void SetColor(Color color) => _bodyMaterial.SetColor("_Color", color);
    private void SetFloat(float value) => _bodyMaterial.SetFloat("_Threshold", value);

    public void BlinkColor(Color color)
    {
        _blinkSequence?.Kill();

        _blinkSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.To(() => Color.black, SetColor, color, 0.1f))
            .Append(DOTween.To(() => color, SetColor, Color.black, 0.15f));
    }
    
    public void BlinkColor(Color color, int loop)
    {
        _blinkSequence?.Kill();

        _blinkSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.To(() => Color.black, SetColor, color, 0.1f))
            .Append(DOTween.To(() => color, SetColor, Color.black, 0.15f))
            .SetLoops(loop);
    }

    public void ShakeBody(float strength = 0.25f, int vibrato = 30)
    {
        _shakeSequence?.Kill();

        _shakeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.Shake(() => Vector3.zero, offset => _shakeOffset = offset, 0.5f, strength, vibrato))
            .OnUpdate(() => _bodyTransform.localPosition += _shakeOffset)
            .SetUpdate(UpdateType.Late);
    }

    public void ShakeBody(int loop)
    {
        _shakeSequence?.Kill();
        
        _shakeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.Shake(() => Vector3.zero, offset => _shakeOffset = offset, _attackReadyShakeDuration, _attackReadyShakeStrength, _attackReadyShakeVibrato, fadeOut: false))
            .OnUpdate(() => _bodyTransform.localPosition += _shakeOffset)
            .SetUpdate(UpdateType.Late)
            .SetLoops(loop);
    }

    public void Disovle(float delay = 0, Action onComplete = null)
    {
        _skinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _disovleSequence?.Kill();

        _disovleSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .AppendInterval(delay)
            .Append(DOTween.To(() => 0f, SetFloat, 1f, 2f))
            .AppendCallback(() => onComplete?.Invoke());
    }

    public async UniTask HitStop(Animator animator, float duration, Action onComplete = null)
    {
        animator.speed = 0f;
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        animator.speed = 1f;
        onComplete?.Invoke();
    } 
}
