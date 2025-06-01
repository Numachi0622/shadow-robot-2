using System;
using Cysharp.Threading.Tasks;
using Interface;
using Player;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] private PlayerParams _params;
    [SerializeField] private PlayerAttacker _leftAttacker, _rightAttacker;
    [SerializeField] private Transform _leftHand, _rightHand;
    [SerializeField] private PlayerKinectMotion _kinectMotion;
    [SerializeField] private HitPointPresenter _hpPresenter;
    [SerializeField] private PlayerEffect _playerEffect;
    [SerializeField] private RocketPunchDetector _rocketPunchDetector;
    [SerializeField] private Collider _takeDamageCollider;
    [SerializeField] private FollowingCamera _followingCamera;
    
    private HandAttackDetector _leftHandAttackDetector, _rightHandAttackDetector;
    public void Initialize()
    {
        // Initialize
        _leftHandAttackDetector = new HandAttackDetector(_leftHand, _params.AttackableVelocity);
        _rightHandAttackDetector = new HandAttackDetector(_rightHand, _params.AttackableVelocity);
        _rocketPunchDetector.Initialize();
        _kinectMotion.Initialize();
        _hpPresenter.Initialize(_params);
        _leftAttacker.Initialize(_params);
        _rightAttacker.Initialize(_params);
        _playerEffect.Initialize();
        
        // Bind
        Bind();
        
        // Set Events
        SetEvents();
    }

    private void Bind()
    {
        // Attack
        _leftHandAttackDetector.OnAttack
            //.Where(_ => !_kinectMotion.IsJumping)
            .Subscribe(info => _leftAttacker.Attack(info.dir, info.vel))
            .AddTo(this);
        
        _leftHandAttackDetector.OnAttackEnd
            .Subscribe(_ => _leftAttacker.AttackEnd())
            .AddTo(this);

        _rightHandAttackDetector.OnAttack
            //.Where(_ => !_kinectMotion.IsJumping)
            .Subscribe(info => _rightAttacker.Attack(info.dir, info.vel))
            .AddTo(this);
        
        _rightHandAttackDetector.OnAttackEnd
            .Subscribe(_ => _rightAttacker.AttackEnd())
            .AddTo(this);

        _rocketPunchDetector.OnReadyRocketPunch
            .Subscribe(_ => _followingCamera.SetRocketPunchCamera())
            .AddTo(this);
        
        _rocketPunchDetector.OnResetRocketPunch
            .Subscribe(_ => _followingCamera.CancelRocketPunchCamera())
            .AddTo(this);
        
        // Take Damage
        this.OnTriggerEnterAsObservable()
            .Select(hitCollider =>
            {
                if (hitCollider.TryGetComponent<IAttackable>(out var attacker))
                {
                    return (attacker, hitCollider);
                }
                
                return (null, null);
            })
            .Where(attacker => attacker.attacker != null)
            .Select(attacker =>
            {
                var damageInfo = attacker.attacker.AttackInfo;
                var targetTransform = attacker.hitCollider.transform;
                var hitPos = _takeDamageCollider.ClosestPoint(targetTransform.position);
                return (damageInfo, hitPos);
            })
            .Subscribe(info =>
            {
                var damageInfo = info.damageInfo;
                var damage = damageInfo.AttackPoint.RandomValue;
                
                _hpPresenter.DecreaseHp(damage);
                _playerEffect.BlinkColor(_params.DamagedColor);
                DamageTextView.Instance.Play(damageInfo.AttackType, damage, info.hitPos);
                HitEffectManager.Instance.Play(damageInfo.AttackType, info.hitPos);
            })
            .AddTo(this);
    }

    private void SetEvents()
    {
        _hpPresenter.OnHpDeleted(() =>
        {
            _takeDamageCollider.enabled = false;
            GameStatePresenter.Instance.SetState(GameState.GameOver);
        });
    }
}
