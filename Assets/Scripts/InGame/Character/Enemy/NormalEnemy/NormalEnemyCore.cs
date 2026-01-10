using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Message;
using InGame.System;
using MessagePipe;
using UniRx;
using UnityEngine;
using VContainer;

namespace InGame.Character
{
    public class NormalEnemyCore : EnemyCore
    {
        [Serializable]
        public class NormalEnemyEffectComponents
        {
            public ParticleSystem AttackEffect;
        }

        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private NormalEnemyEffectComponents _effectComponents;
        [SerializeField] private HitPointPresenter _hpPresenter;

        private StateMachine<NormalEnemyCore> _stateMachine;
        private NormalEnemyAttackObserver _attackObserver;
        private NormalEnemyMoveObserver _moveObserver;
        private EnemyEffect _enemyEffect;
        private Transform _targetTransform;
        private CancellationTokenSource _cancellationTokenSource;
        private CharacterRegistry _characterRegistry;
        private IPublisher<EnemyDestroyedMessage> _enemyDestroyedPublisher;
        private ISubscriber<AllEnemyDespawnMessage> _allEnemyDespawnSubscriber;
        private AreaId _areaId;

        public EnemyParams Params => _params;
        public IMovable Mover => _mover;
        public NormalEnemyEffectComponents Effect => _effectComponents;
        public EnemyEffect EnemyEffect => _enemyEffect;
        public Animator Animator => _animator;

        private bool IsIdle => _stateMachine.CurrentState is NormalEnemyIdleState;
        private bool IsMoving => _stateMachine.CurrentState is NormalEnemyMoveState;
        private bool IsDead => _stateMachine.CurrentState is NormalEnemyDeadState;

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
        
        [Inject]
        public void Construct(
            CharacterRegistry characterRegistry,
            IPublisher<EnemyDestroyedMessage> enemyDestroyedPublisher,
            ISubscriber<AllEnemyDespawnMessage> allEnemyDespawnSubscriber)
        {
            _characterRegistry = characterRegistry;
            _enemyDestroyedPublisher = enemyDestroyedPublisher;
            _allEnemyDespawnSubscriber = allEnemyDespawnSubscriber;
        }

        public override void Initialize()
        {
            base.Initialize();
            _attacker = new NormalEnemyAttacker(_params, _attackCollider);
            _damager = new Damager(_hpPresenter);
            _mover = new NormalEnemyMover(transform);
            _enemyEffect = new EnemyEffect(_params, transform, _bodyTransform);

            _stateMachine = new StateMachine<NormalEnemyCore>(this);
            _attackObserver = new NormalEnemyAttackObserver(
                transform,
                _params.AttackRange,
                _params.AttackReadyTime
            );
            _moveObserver = new NormalEnemyMoveObserver(
                transform,
                _params.SearchRange,
                _params.AttackRange
            );
            
            _hpPresenter.Initialize(null, _params);
            _stateMachine.SetState<NormalEnemyIdleState>();
            
            Bind();
        }
        
        public void SetAreaId(AreaId areaId)
        {
            _areaId = areaId;
        }

        private void Bind()
        {
            _moveObserver.OnMoveStart
                .Subscribe(OnMoveStart)
                .AddTo(this);

            _moveObserver.OnMoveEnd
                .Subscribe(OnIdleStart)
                .AddTo(this);
            
            _attackObserver.OnAttackStart
                .Subscribe(OnAttackReadyStart)
                .AddTo(this);

            _hpPresenter.OnHpDecreased
                .Subscribe(OnDeadStart)
                .AddTo(this);

            _damageObserver.OnTakeDamage
                .Subscribe(OnDamageStart)
                .AddTo(this);
            
            // 外部から強制的にデスポーン（場面切り替えなど）
            _allEnemyDespawnSubscriber.Subscribe(_ =>
            {
                // todo: エフェクト再生したい
                enabled = false;
                _enemyDestroyedPublisher.Publish(new EnemyDestroyedMessage(_areaId, this));
                Destroy(gameObject);
            }).AddTo(this);
        }

        public override void OnUpdate()
        {
            _targetTransform = _characterRegistry.GetNearestBuilding(_areaId, transform.position)?.transform;
            if (_targetTransform == null) return;
            
            var dest = _targetTransform.position;
            if (IsIdle || IsMoving)
            {
                _moveObserver.Observe(dest);
            }
            
            if (IsIdle)
            {
                _attackObserver.Observe(dest);
            }
            
            _stateMachine.OnUpdate(new EnemyMoveParams(){ Destination = dest});
        }

        #region State Event
        public void OnIdleStart(Unit unit)
        {
            if (IsDead) return;
            _stateMachine.SetState<NormalEnemyIdleState>();
        }   
        private void OnMoveStart(Unit unit)
        {
            _stateMachine.SetState<NormalEnemyMoveState>();
        }
        private void OnAttackReadyStart(AttackReadyParam param)
        {
            _stateMachine.SetState<NormalEnemyAttackReadyState>(param);
        }
        public void OnAttackStart(AttackReadyParam param)
        {
            _stateMachine.SetState<NormalEnemyAttackState>(param);
        }

        public void OnCoolTimeStart()
        {
            _stateMachine.SetState<NormalEnemyCoolTimeState>();
        }
        
        private void OnDamageStart(AttackParam param)
        {
            _stateMachine.SetState<NormalEnemyDamageState>(param);
        }
        
        private void OnDeadStart(Unit unit)
        {
            _stateMachine.SetState<NormalEnemyDeadState>();
            _enemyDestroyedPublisher.Publish(new EnemyDestroyedMessage(_areaId, this));
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