using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Message;
using InGame.System;
using MessagePipe;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace InGame.Character
{
    public class NormalEnemyCore : EnemyCore
    {
        [Serializable]
        public class NormalEnemyEffectComponents
        {
            public ParticleSystem LaserEffect;
            public ParticleSystem LongLaserEffect;
        }

        [SerializeField] private EnemyParams _laserOnlyParams;
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
        private ISubscriber<AllEnemyStopMessage> _allEnemyStopSubscriber;
        private AreaId _areaId;
        private TargetType _targetType;
        private bool _isForceStopped;
        private PlayType _playType;
        private CharacterId _lastAttackerId;

        public EnemyParams Params => _params;
        public IMovable Mover => _mover;
        public NormalEnemyEffectComponents Effect => _effectComponents;
        public EnemyEffect EnemyEffect => _enemyEffect;
        public Animator Animator => _animator;
        public PlayType PlayType => _playType;
        public CharacterRegistry CharacterRegistry => _characterRegistry;

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
            ISubscriber<AllEnemyDespawnMessage> allEnemyDespawnSubscriber,
            ISubscriber<AllEnemyStopMessage> allEnemyStopSubscriber)
        {
            _characterRegistry = characterRegistry;
            _enemyDestroyedPublisher = enemyDestroyedPublisher;
            _allEnemyDespawnSubscriber = allEnemyDespawnSubscriber;
            _allEnemyStopSubscriber = allEnemyStopSubscriber;
        }

        public override void Initialize()
        {
            base.Initialize();
            _attacker = new NormalEnemyAttacker(_params, _attackCollider);
            _damager = new Damager(_hpPresenter);
            _mover = new NormalEnemyMover(transform);
            _enemyEffect = new EnemyEffect(_params, transform, _center);

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

        public void SetTarget(TargetType targetType)
        {
            _targetType = targetType;
        }
        
        public void SetPlayType(PlayType playType)
        {
            _playType = playType;
            if (_playType == PlayType.Multi)
            {
                _params = _laserOnlyParams;
                _attackObserver.SetParam(_params.AttackRange, _params.AttackReadyTime);
                _moveObserver.SetParam(_params.SearchRange, _params.AttackRange);
            }
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

            // 外部から強制的に停止（ゲームオーバーなど）
            _allEnemyStopSubscriber.Subscribe(_ => ForceStop()).AddTo(this);
        }

        public override void OnUpdate()
        {
            if (_isForceStopped) return;

            _targetTransform = _targetType == TargetType.Building
                ? _characterRegistry.GetNearestBuilding(_areaId, transform.position)?.transform
                : _characterRegistry.GetAllPlayers().FirstOrDefault()?.transform;
            if (_targetTransform == null) return;
            
            var dest = _targetTransform.position;
            _stateMachine.OnUpdate(new EnemyMoveParams(){ Destination = dest });
            
            if (IsIdle || IsMoving)
            {
                _moveObserver.Observe(dest);
            }
            
            if (IsIdle)
            {
                _attackObserver.Observe(dest);
            }
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
            _lastAttackerId = param.AttackerId;
            _stateMachine.SetState<NormalEnemyDamageState>(param);
        }

        private void OnDeadStart(Unit unit)
        {
            _stateMachine.SetState<NormalEnemyDeadState>();
            _enemyDestroyedPublisher.Publish(new EnemyDestroyedMessage(_areaId, this, _lastAttackerId));
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

        protected override void ForceStop()
        {
            _isForceStopped = true;
            _stateMachine.SetState<NormalEnemyIdleState>();
        }
    }
}