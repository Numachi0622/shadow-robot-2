using InGame.System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyCore : EnemyCore
    {
        [SerializeField] private HitPointPresenter _hpPresenter;

        private StateMachine<NormalEnemyCore> _stateMachine; 
        private NormalEnemyAttackObserver _attackObserver;
        private NormalEnemyMoveObserver _moveObserver;
        private Transform _targetTransform;
        
        public EnemyParams Params => _params;
        public IMovable Mover => _mover;
        
        private bool IsIdle => _stateMachine.CurrentState is NormalEnemyIdleState;
        private bool IsMoving => _stateMachine.CurrentState is NormalEnemyMoveState;
        
        public override void Initialize()
        {
            _attacker = new NormalEnemyAttacker(_params, _attackCollider);
            _damager = new Damager(_hpPresenter);
            _mover = new NormalEnemyMover(transform);

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
            
            //_hpPresenter.Initialize(_params);
            
            _stateMachine.SetState<NormalEnemyIdleState>();
            
            // debug
            _targetTransform = GameObject.Find("Building").transform;
            
            Bind();
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
        }

        public override void OnUpdate()
        {
            // var target = CharacterRegistry.GetNearestBuilding(transform.position);
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
            _stateMachine.SetState<NormalEnemyIdleState>();
        }
        private void OnMoveStart(Unit unit)
        {
            _stateMachine.SetState<NormalEnemyMoveState>();
        }
        private void OnAttackReadyStart(EnemyAttackParam param)
        {
            _stateMachine.SetState<NormalEnemyAttackReadyState>(param);
        }
        public void OnAttackStart(EnemyAttackParam param)
        {
            _stateMachine.SetState<NormalEnemyAttackState>(param);
        }

        public void OnCoolTimeStart()
        {
            _stateMachine.SetState<NormalEnemyCoolTimeState>();
        }
        #endregion
    }
}