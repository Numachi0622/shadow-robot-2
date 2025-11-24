using System;
using System.Threading;
using InGame.System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class BossEnemyCore : EnemyCore
    {
        [Serializable]
        public class BossEnemyEffectComponents
        {
            public ParticleSystem FireBallEffectPrefab;
            public ParticleSystem DeathBallEffectPrefab;
        }
        
        [SerializeField] private CharacterCore _normalEnemyPrefab;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private BossEnemyEffectComponents _effectComponents;
        [SerializeField] private HitPointPresenter _hpPresenter;

        private StateMachine<BossEnemyCore> _stateMachine;
        private BossEnemyAttackObserver _attackObserver;
        private EnemyEffect _enemyEffect;
        private Transform _targetTransform;
        private CancellationTokenSource _cancellationTokenSource;
        
        public EnemyParams Params => _params;
        public EnemyEffect EnemyEffect => _enemyEffect;
        public Animator Animator => _animator;
        public CharacterCore NormalEnemyPrefab => _normalEnemyPrefab;
        public BossEnemyEffectComponents EffectComponents => _effectComponents;
        
        public CancellationTokenSource CancellationTokenSource
        {
            get
            {
                if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                }
                return _cancellationTokenSource;
            }
        }
        
        private bool IsIdle => _stateMachine.CurrentState is BossEnemyIdleState;
        private bool IsDead => _stateMachine.CurrentState is BossEnemyDeadState;

        public override void Initialize() 
        {
            base.Initialize();

            _attacker = new EnemyAttacker(_params, _attackCollider);
            _damager = new Damager(_hpPresenter);
            _enemyEffect = new EnemyEffect(_params, transform, _bodyTransform);

            _stateMachine = new StateMachine<BossEnemyCore>(this);
            _attackObserver = new BossEnemyAttackObserver(
                transform,
                3f,
                _params.AttackReadyTime);
            
            _hpPresenter.Initialize(_params);
            _stateMachine.SetState<BossEnemyIdleState>();
            
            Bind();
        }

        private void Bind()
        {
            _attackObserver.OnAttackStart
                .Subscribe(OnAttackReadyStart)
                .AddTo(this);

            _hpPresenter.OnHpDecreased
                .Subscribe(OnDeadStart)
                .AddTo(this);

            _damageObserver.OnTakeDamage
                .Subscribe(OnDamageStart)
                .AddTo(this);
        }
        
        public override void OnUpdate()
        {
            _targetTransform = CharacterRegistry.GetNearestPlayer(transform.position).transform;

            var dest = _targetTransform.position;
            if (IsIdle)
            {
                _attackObserver.Observe(dest);
            }
            
            _stateMachine.OnUpdate();
        }

        #region State Event
        public void OnIdleStart(Unit unit)
        {
            if (IsDead) return;
            _stateMachine.SetState<BossEnemyIdleState>();
        }
        private void OnAttackReadyStart(AttackReadyParam param)
        {
            _stateMachine.SetState<BossEnemyAttackReadyState>(param);
        }
        public void OnAttackStart(AttackReadyParam param)
        {
            _stateMachine.SetState<BossEnemyAttackState>(param);
        }
        public void OnCoolTimeStart()
        {
            _stateMachine.SetState<BossEnemyCoolTimeState>();
        }
        private void OnDamageStart(AttackParam param)
        {
            _stateMachine.SetState<BossEnemyDamageState>(param);
        }
        private void OnDeadStart(Unit unit)
        {
            _stateMachine.SetState<BossEnemyDeadState>();
            Destroy(gameObject, 3f);
        }
        #endregion
        
        public void AttackCancel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void AnimationEventTemp()
        {
        }
    }
}