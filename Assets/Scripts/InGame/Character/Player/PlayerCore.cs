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

        private StateMachine<PlayerCore> _stateMachine;
        private IAttackable _leftHandAttacker, _rightHandAttacker;
        private HandAttackObserver _leftHandObserver, _rightHandObserver;
        private DamageObserver _damageObserver;
        private PlayerMotionMover _motionMover;
        private SynMotionSystem _synMotion;
        private int _playerId = 0;
        
        public IAttackable LeftHandAttacker => _leftHandAttacker;
        public IAttackable RightHandAttacker => _rightHandAttacker;

        public override void Initialize(int id, SynMotionSystem synMotion)
        {
            _playerId = id;
            _synMotion = synMotion;

            _leftHandAttacker = new PlayerAttacker(_params, _leftHandCollider);
            _rightHandAttacker = new PlayerAttacker(_params, _rightHandCollider);
            _mover = new PlayerMover(_movementTransforms.Reference);
            _motionMover = new PlayerMotionMover(_movementTransforms);

            Bind();
        }

        private void Bind()
        {
            _leftHandObserver.OnAttackStart
                .Merge(_rightHandObserver.OnAttackStart)
                .Subscribe(OnAttackStart)
                .AddTo(this);

            _leftHandObserver.OnAttackEnd
                .Merge(_rightHandObserver.OnAttackEnd)
                .Subscribe(_ => OnIdleStart())
                .AddTo(this);

            _damageObserver.OnTakeDamage
                .Subscribe(OnDamageStart)
                .AddTo(this);
        }

        public override void OnUpdate()
        {
            _leftHandObserver.Observe();
            _rightHandObserver.Observe();

            var motionParam = _synMotion.GetMotionParam(_playerId);
            _mover.Move(motionParam.SpineMidPosition);
            _motionMover.UpdateMotion(motionParam);
        }

        private void OnIdleStart()
        {
            _stateMachine.SetState<PlayerIdleState>();
        }
        
        private void OnAttackStart(HandAttackParam param)
        {
            _stateMachine.SetState<PlayerAttackState>(param);
        }

        private void OnDamageStart(AttackParam param)
        {
            _stateMachine.SetState<PlayerDamageState>(param);
        }
    }
}
