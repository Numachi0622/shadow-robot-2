using DG.Tweening;
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
    
    public void Initialize()
    {
        _bodyMaterial = _skinnedMeshRenderer.material;
    }

    private void SetColor(Color color) => _bodyMaterial.SetColor("_Color", color);

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
}
