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
    [SerializeField] private Attacker _leftAttacker, _rightAttacker;
    [SerializeField] private Transform _leftHand, _rightHand;
    [SerializeField] private PlayerKinectMotion _kinectMotion;
    [SerializeField] private HitPointPresenter _hpPresenter;
    private HandAttackDetector _leftHandAttackDetector, _rightHandAttackDetector;
    public void Initialize()
    {
        // Initialize
        _leftHandAttackDetector = new HandAttackDetector(_leftHand, _params.AttackableVelocity);
        _rightHandAttackDetector = new HandAttackDetector(_rightHand, _params.AttackableVelocity);
        _kinectMotion.Initialize();
        _hpPresenter.Initialize(_params);
        _leftAttacker.Initialize(_params);
        _rightAttacker.Initialize(_params);
        
        // Bind
        Bind();
        
        // Set Events
        SetEvents();
    }

    private void Bind()
    {
        // Attack
        _leftHandAttackDetector.OnAttack
            .Subscribe(info => _leftAttacker.Attack(info.dir, info.vel))
            .AddTo(this);
        
        _leftHandAttackDetector.OnAttackEnd
            .Subscribe(_ => _leftAttacker.AttackEnd())
            .AddTo(this);

        _rightHandAttackDetector.OnAttack
            .Subscribe(info => _rightAttacker.Attack(info.dir, info.vel))
            .AddTo(this);
        
        _rightHandAttackDetector.OnAttackEnd
            .Subscribe(_ => _rightAttacker.AttackEnd())
            .AddTo(this);
        
        // Take Damage
        this.OnTriggerEnterAsObservable()
            .Select(hitCollider =>
            {
                if (TryGetComponent<IAttackable>(out var attacker))
                {
                    return attacker;
                }
                
                return null;
            })
            .Where(attacker => attacker != null)
            .Select(attacker => attacker.AttackInfo)
            .Subscribe(damageInfo =>
            {
                _hpPresenter.DecreaseHp(damageInfo.AttackPoint.RandomValue);
            })
            .AddTo(this);
        
        // debug
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.D))
            .Subscribe(_ => _hpPresenter.DecreaseHp(10))
            .AddTo(this);
    }

    public void SetEvents()
    {
        _hpPresenter.OnHpDeleted(() => Debug.Log("Dead"));
    }
}
