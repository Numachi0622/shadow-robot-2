using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.Character;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EnemyEffect
{
    private EnemyParams _params;
    private Transform _rootTransform;
    private Transform _bodyTransform;
    //[SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    private float _attackReadyShakeStrength = 0.06f;
    private int _attackReadyShakeVibrato = 20;
    private float _attackReadyShakeDuration = 0.5f;

    private Material _bodyMaterial;
    private Vector3 _shakeOffset;
    
    private Sequence _blinkSequence;
    private Sequence _shakeSequence;
    private Sequence _disovleSequence;
    private Sequence _knockBackSequence;

    public EnemyEffect(EnemyParams param, Transform root, Transform body)
    {
        _params = param;
        _rootTransform = root;
        _bodyTransform = body;
    }
    

    private void SetColor(Color color) => _bodyMaterial.SetColor("_Color", color);
    private void SetFloat(float value) => _bodyMaterial.SetFloat("_Threshold", value);

    public void BlinkColor(Color color)
    {
        // _blinkSequence?.Kill();
        //
        // _blinkSequence = DOTween.Sequence()
        //     .SetLink(gameObject)
        //     .Append(DOTween.To(() => Color.black, SetColor, color, 0.1f))
        //     .Append(DOTween.To(() => color, SetColor, Color.black, 0.15f));
    }
    
    public void BlinkColor(Color color, int loop)
    {
        // _blinkSequence?.Kill();
        //
        // _blinkSequence = DOTween.Sequence()
        //     .SetLink(gameObject)
        //     .Append(DOTween.To(() => Color.black, SetColor, color, 0.1f))
        //     .Append(DOTween.To(() => color, SetColor, Color.black, 0.15f))
        //     .SetLoops(loop);
    }

    /// <summary>
    /// 振動アニメーション再生
    /// ダメージを受けたときに使用
    /// </summary>
    public void ShakeBody(float strength = 0.15f, int vibrato = 30)
    {
        _shakeSequence?.Kill();
        
        _shakeSequence = DOTween.Sequence()
            .SetLink(_rootTransform.gameObject)
            .Append(DOTween.Shake(
                () => Vector3.zero,
                offset => _shakeOffset = offset,
                _params.EffectParams.KnockBackDuration,
                strength,
                vibrato,
                fadeOut: true))
            .Append(DOTween.To(
                () => _shakeOffset,
                offset => _shakeOffset = offset,
                Vector3.zero,
                0.3f
            ).SetEase(Ease.OutQuad))
            .OnUpdate(() => _bodyTransform.localPosition += _shakeOffset)
            .SetUpdate(UpdateType.Late);
    }

    /// <summary>
    /// 振動アニメーション再生
    /// AttackReadyの予備動作として使用
    /// </summary>
    /// <param name="loop"></param>
    public void ShakeBody(int loop)
    {
        _shakeSequence?.Kill();
        
        _shakeSequence = DOTween.Sequence()
            .SetLink(_rootTransform.gameObject)
            .Append(DOTween.Shake(
                () => Vector3.zero,
                offset => _shakeOffset = offset, 
                _attackReadyShakeDuration, 
                _attackReadyShakeStrength, 
                _attackReadyShakeVibrato, 
                fadeOut: false))
            .Append(DOTween.To(
                () => _shakeOffset,
                offset => _shakeOffset = offset,
                Vector3.zero,
                0.05f
                ))
            .OnUpdate(() => _bodyTransform.localPosition += _shakeOffset)
            .SetUpdate(UpdateType.Late)
            .SetLoops(loop);
    }

    public void Disovle(float delay = 0, Action onComplete = null)
    {
        // _skinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        // _disovleSequence?.Kill();
        //
        // _disovleSequence = DOTween.Sequence()
        //     .SetLink(gameObject)
        //     .AppendInterval(delay)
        //     .Append(DOTween.To(() => 0f, SetFloat, 1f, 2f))
        //     .AppendCallback(() => onComplete?.Invoke());
    }

    public async UniTask HitStop(Animator animator, float duration, Action onComplete = null)
    {
        animator.speed = 0f;
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        animator.speed = 1f;
        onComplete?.Invoke();
    } 
    
    public void KnockBack(Vector3 dir, bool isDead = false)
    {
        _knockBackSequence?.Kill();

        var dist = isDead ? _params.EffectParams.DeadKnockBackDistance : _params.EffectParams.KnockBackDistance;
        var currentHeight = _rootTransform.position.y;
        var destination = _rootTransform.position + dir * dist;
        destination.y = currentHeight;
        
        _knockBackSequence = DOTween.Sequence()
            .SetLink(_rootTransform.gameObject)
            .Append(_rootTransform.DOMove(destination, _params.EffectParams.KnockBackDuration).SetEase(Ease.OutCubic));
    }
}
