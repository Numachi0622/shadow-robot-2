using System;
using System.Linq;
using System.Threading;
using InGame.Message;
using InGame.System;
using InGame.System.UI;
using MessagePipe;
using UniRx;
using UnityEngine;
using VContainer;

namespace InGame.Character
{
    public class BossEnemyCore : EnemyCore
    {
        [Serializable]
        public class BossEnemyEffectComponents
        {
            public ParticleSystem DeathBallAura;
        }
        
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private HitPointPresenter _hpPresenter;
        [SerializeField] private BossEnemyEffectComponents _effectComponents;
        [SerializeField] private Transform _center;
        [SerializeField] private Transform _fireBallFirePoint;
        [SerializeField] private Transform _fireHomingFirePoint;
        [SerializeField] private Transform _deathBallFirePoint;

        private HitPointView _hpView;
        private StateMachine<BossEnemyCore> _stateMachine;
        private BossEnemyAttackObserver _attackObserver;
        private EnemyEffect _enemyEffect;
        private Transform _targetTransform;
        private CancellationTokenSource _cancellationTokenSource;
        private CharacterRegistry _characterRegistry;
        private IPublisher<EnemyDestroyedMessage> _enemyDestroyedPublisher;
        private IPublisher<StateChangeMessage> _stateChangePublisher;
        private ISubscriber<AllEnemyStopMessage> _allEnemyStopSubscriber;
        private ISubscriber<BossBattleStartMessage> _bossBattleStartSubscriber;
        
        public EnemyParams Params => _params;
        public EnemyEffect EnemyEffect => _enemyEffect;
        public Animator Animator => _animator;
        public BossEnemyEffectComponents EffectComponents => _effectComponents;
        public Transform FireBallFirePoint => _fireBallFirePoint;
        public Transform FireHomingFirePoint => _fireHomingFirePoint;
        public Transform DeathBallFirePoint => _deathBallFirePoint;
        public Transform TargetTransform => (_characterRegistry.GetAllPlayers().FirstOrDefault() as PlayerCore)?.Center;
        public HitPointPresenter HpPresenter => _hpPresenter;
        public Transform Center => _center;

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
        public IPublisher<SpawnCharacterMessage> SummonEnemyPublisher { get; private set; }
        public IPublisher<AllEnemyDespawnMessage> AllEnemyDespawnPublisher { get; private set; }
        public IPublisher<PoseMatchEventStartMessage> PoseMatchEventStartPublisher { get; private set; }
        public ISubscriber<PoseMatchEventEndMessage> PoseMatchEventEndSubscriber { get; private set; }
        public IPublisher<ShowWarningMessage> ShowWarningPublisher { get; private set; }
        
        private bool _isForceStopped;
        private bool _isInitialized;

        private bool IsIdle => _stateMachine.CurrentState is BossEnemyIdleState;
        private bool IsDead => _stateMachine.CurrentState is BossEnemyDeadState;
        
        [Inject]
        public void Construct(
            HitPointViewList hitPointViewList,
            CharacterRegistry characterRegistry,
            IPublisher<SpawnCharacterMessage> summonEnemyPublisher,
            IPublisher<AllEnemyDespawnMessage> allEnemyDespawnPublisher,
            IPublisher<PoseMatchEventStartMessage> poseMatchEventStartPublisher,
            ISubscriber<PoseMatchEventEndMessage> poseMatchEventEndSubscriber,
            IPublisher<EnemyDestroyedMessage> enemyDestroyedPublisher,
            IPublisher<StateChangeMessage> stateChangePublisher,
            ISubscriber<AllEnemyStopMessage> allEnemyStopSubscriber,
            IPublisher<ShowWarningMessage> showWarningPublisher,
            ISubscriber<BossBattleStartMessage> bossBattleStartSubscriber)
        {
            _hpView = hitPointViewList.BossHitPointView;
            _characterRegistry = characterRegistry;
            SummonEnemyPublisher = summonEnemyPublisher;
            AllEnemyDespawnPublisher = allEnemyDespawnPublisher;
            PoseMatchEventStartPublisher = poseMatchEventStartPublisher;
            PoseMatchEventEndSubscriber = poseMatchEventEndSubscriber;
            _enemyDestroyedPublisher = enemyDestroyedPublisher;
            _stateChangePublisher = stateChangePublisher;
            _allEnemyStopSubscriber = allEnemyStopSubscriber;
            ShowWarningPublisher = showWarningPublisher;
            _bossBattleStartSubscriber = bossBattleStartSubscriber;
        }

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
            
            _hpPresenter.Initialize(_hpView, _params);
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
            
            // 外部から強制的に停止させる
            _allEnemyStopSubscriber.Subscribe(_ => ForceStop()).AddTo(this);

            _bossBattleStartSubscriber.Subscribe(_ => _isInitialized = true).AddTo(this);
        }
        
        public override void OnUpdate()
        {
            if (_isForceStopped) return;
            if (!_isInitialized) return;

            _targetTransform = _characterRegistry.GetNearestPlayer(transform.position)?.transform;
            if (_targetTransform == null) return;

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
            _enemyDestroyedPublisher.Publish(new EnemyDestroyedMessage(new AreaId(-1), this));

            var resultContext = new ResultContextMessage(_characterRegistry.TotalBuildingCount);
            _stateChangePublisher.Publish(new StateChangeMessage(GameStateType.Result, resultContext));
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
            _stateMachine.SetState<BossEnemyIdleState>();
        }
    }
}