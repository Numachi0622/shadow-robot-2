using System;
using Interface;
using UniRx;
using UniRx.Triggers;
using UnityEditor.Recorder;
using UnityEngine;
using Utility;

public class RocketPunchAttacker : MonoBehaviour, IAttackable
{
    [SerializeField] private AttackPoint _rocketPunchAttackPoint;
    [SerializeField] private float _launchSpeed = 10f;
    [SerializeField] private float _returnSpedd = 50f;
    [SerializeField] private float _returnableRange = 2f;
    [SerializeField] private float _maxLaunchDistance = 100f;
    [SerializeField] private ParticleSystem _hitEffect;
    private Transform _root;
    private bool _isHit = false;
    private bool _isSepareted = false;
    private AttackInfo _attackInfo;
    public AttackInfo AttackInfo => _attackInfo;
    private Vector3 RootPos => _root.position + Vector3.up;
    

    public void Initialize(Transform root)
    {
        _root = root;
        _attackInfo = new AttackInfo()
        {
            AttackPoint = _rocketPunchAttackPoint,
            AttackDirection = Vector3.zero,
            AttackVelocity = 0f,
            AttackType = AttackType.PlayerToEnemyNormal
        };

        this.OnTriggerEnterAsObservable()
            .Subscribe(_ =>
            {
                _isHit = true;
                var hitEffect = Instantiate(_hitEffect, transform.position, Quaternion.identity);
                SoundManager.Instance.PlaySe(11);
            })
            .AddTo(this);
    }

    public void Execute(Vector3 dir, Action onComplete = null)
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                var dist = Vector3.Distance(transform.position, RootPos);
                if (dist > _maxLaunchDistance)
                {
                    _isSepareted = true;
                }

                if (_isSepareted || _isHit)
                {
                    transform.LookAt(RootPos);
                    var returnedPos = Vector3.MoveTowards(transform.position, RootPos, _returnSpedd * Time.deltaTime);
                    transform.position = returnedPos;
                    
                    dist = Vector3.Distance(transform.position, RootPos);
                    if(dist < _returnableRange)
                    {
                        onComplete?.Invoke();
                        Destroy(gameObject);
                    }
                    
                    return;
                }
                
                transform.LookAt(transform.position + dir);
                var movedPos = transform.position + dir * (_launchSpeed * Time.deltaTime);
                transform.position = movedPos;
            })
            .AddTo(this);
    }
    
    public void Attack(float waitTime = 0)
    {
    }

    public void Attack(Vector3 dir, float velocity)
    {
    }

    public void AttackEnd()
    {
    }
}
