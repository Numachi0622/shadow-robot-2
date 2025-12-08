using System;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.System;
using SynMotion;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerCore : CharacterCore
    {
        [SerializeField] private PlayerParams _params;

        [Serializable]
        public class MovementTransforms
        {
            public Transform Reference;
            public Transform LeftUpLeg;
            public Transform LeftLeg;
            public Transform RightUpLeg;
            public Transform RightLeg;
            public Transform FirstSpine;
            public Transform LeftArm;
            public Transform LeftForeArm;
            public Transform LeftHand;
            public Transform RightArm;
            public Transform RightForeArm;
            public Transform RightHand;
        }

        [SerializeField] private MovementTransforms _movementTransforms;

        [SerializeField] private AttackCollider _leftHandCollider;
        [SerializeField] private AttackCollider _rightHandCollider;
        [SerializeField] private DamageCollider _damageCollider;
        [SerializeField] private HitPointPresenter _hpPresenter;
        [SerializeField] private Camera _playerCamera;

        private StateMachine<PlayerCore> _stateMachine;
        private IAttackable _leftHandAttacker, _rightHandAttacker;
        private HandAttackObserver _leftHandObserver, _rightHandObserver;
        private DamageObserver _damageObserver;
        private PlayerMotionMover _motionMover;
        private SynMotionSystem _synMotion;
        private CharacterId _playerId;
        private bool _isMovable = false;
        
        public CharacterId PlayerId => _playerId;
        
        public IAttackable LeftHandAttacker => _leftHandAttacker;
        public IAttackable RightHandAttacker => _rightHandAttacker;
        public IDamageable Damager => _damager;

        public override void Initialize(CharacterId id, SynMotionSystem synMotion)
        {
            _damager = new Damager(_hpPresenter);
            
            _playerId = id;
            _synMotion = synMotion;
            _stateMachine = new StateMachine<PlayerCore>(this);
            _leftHandObserver = new HandAttackObserver(_movementTransforms.LeftHand, _params.AttackableVelocity);
            _rightHandObserver = new HandAttackObserver(_movementTransforms.RightHand, _params.AttackableVelocity);
            _damageObserver = new DamageObserver();
            _leftHandAttacker = new PlayerAttacker(_params, _leftHandCollider);
            _rightHandAttacker = new PlayerAttacker(_params, _rightHandCollider);
            _mover = new PlayerMover(_movementTransforms.Reference);
            _motionMover = new PlayerMotionMover(_movementTransforms);
            
            _damageCollider.Initialize(_damageObserver);
            _hpPresenter.Initialize(_params);

            Bind();
        }

        private void Bind()
        {
            _leftHandObserver.OnAttackStart
                .Subscribe(OnLeftHandAttackStart)
                .AddTo(this);

            _leftHandObserver.OnAttackEnd
                .Subscribe(OnLeftHandAttackEnd)
                .AddTo(this);
            
            _rightHandObserver.OnAttackStart
                .Subscribe(OnRightHandAttackStart)
                .AddTo(this);

            _rightHandObserver.OnAttackEnd
                .Subscribe(OnRightHAndAttackEnd)
                .AddTo(this);

            _damageObserver.OnTakeDamage
                .Subscribe(OnDamageStart)
                .AddTo(this);

            _hpPresenter.OnHpDecreased
                .Subscribe(OnDeadStart)
                .AddTo(this);
        }

        public override void OnUpdate()
        {
            _leftHandObserver.Observe();
            _rightHandObserver.Observe();

            var motionParam = _synMotion.GetMotionParam(_playerId.Value);
            var pos = _isMovable
                ? motionParam.SpineMidPosition * _params.MoveWeight
                : transform.position + new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (Time.deltaTime * 5f);
            _mover.Move(pos);
            _motionMover.UpdateMotion(motionParam);
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }

        #region Non State Event
        private void OnLeftHandAttackStart(HandAttackParam param)
        {
            _leftHandAttacker.Attack(param.Direction, param.Velocity);
        }
        
        private void OnLeftHandAttackEnd(Unit unit)
        {
            _leftHandAttacker.AttackEnd();
        }
        
        private void OnRightHandAttackStart(HandAttackParam param)
        {
            _rightHandAttacker.Attack(param.Direction, param.Velocity);
        }
        
        private void OnRightHAndAttackEnd(Unit unit)
        {
            _rightHandAttacker.AttackEnd();
        }
        #endregion

        #region State Event
        public void OnIdleStart()
        {
            _stateMachine.SetState<PlayerIdleState>();
        }

        private void OnDamageStart(AttackParam param)
        {
            _stateMachine.SetState<PlayerDamageState>(param);
        }

        private void OnDeadStart(Unit unit)
        {
            _stateMachine.SetState<PlayerDeadState>();
        }
        #endregion
        
        public void SetMovable(bool isMovable)
        {
            _isMovable = isMovable;
        }

        public void SetCamera(bool isActive)
        {
            _playerCamera.gameObject.SetActive(isActive);
        }
    }
}
