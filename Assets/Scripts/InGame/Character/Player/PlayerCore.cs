using System;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using InGame.System;
using MessagePipe;
using SynMotion;
using UniRx;
using UnityEngine;
using VContainer;

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
        private JumpCalibrationSystem _calibrationSystem;
        private bool _isMovable = false;
        private bool _isCalibrated = false;
        private Vector3 _baseWorldPosition;
        private Vector3 _baseTrackingPosition;
        private float _calibratedGroundHeight;
        private bool _isJumping;
        private readonly float _jumpThresholdOffset = 0.2f;
        private readonly float _jumpWeight = 2f;
        
        private ISubscriber<CharacterId, GameStartPlayerInitMessage> _gameStartPlayerInitSubscriber;
        
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
            gameObject.name += _playerId.Value.ToString();

            Bind();

            // キャリブレーション開始
            _calibrationSystem = new JumpCalibrationSystem(_synMotion, _playerId);
            CalibrateAsync().Forget();
        }

        private async UniTaskVoid CalibrateAsync()
        {
            _calibratedGroundHeight = await _calibrationSystem.CalibrateGroundHeight(60);
            _isCalibrated = true;
        }
        
        [Inject]
        public void Construct(
            ISubscriber<CharacterId, GameStartPlayerInitMessage> gameStartPlayerInitSubscriber)
        {
            _gameStartPlayerInitSubscriber = gameStartPlayerInitSubscriber;
        }

        private void Bind()
        {
            // オブザーバーを購読
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
            
            // MessagePipeを購読
            _gameStartPlayerInitSubscriber?.Subscribe(_playerId, OnGameStartInitialize).AddTo(this);
        }

        public override void OnUpdate()
        {
            _leftHandObserver.Observe();
            _rightHandObserver.Observe();

            var motionParam = _synMotion.GetMotionParam(_playerId.Value);
            _motionMover.UpdateMotion(motionParam);

            if (!_isMovable || !_isCalibrated) return; 
            var moveValue = motionParam.SpineMidPosition * _params.MoveWeight - _baseTrackingPosition;
            var movedPos = _baseWorldPosition + moveValue;
            
            var jumpThreshold = _calibratedGroundHeight + _jumpThresholdOffset;
            _isJumping = movedPos.y > jumpThreshold;
            movedPos.y = _isJumping ? (movedPos.y - jumpThreshold) * _jumpWeight : 0f;
            
            // pos = transform.position + new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (Time.deltaTime * 5f);
            _mover.Move(movedPos);
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

        #region MessagePipe Event

        private void OnGameStartInitialize(GameStartPlayerInitMessage message)
        {
            _baseWorldPosition = message.GameStartPosition;
            transform.position = _baseWorldPosition;

            // トラッキング位置を基準位置に設定
            var trackingPos = _synMotion.GetMotionParam(_playerId.Value).SpineMidPosition * _params.MoveWeight;
            _baseTrackingPosition = trackingPos;
            
            SetMovable(true);
            SetCamera(true, message.TotalPlayerCount);
        }     
        
        #endregion
        
        private void SetMovable(bool isMovable)
        {
            _isMovable = isMovable;
        }

        private void SetCamera(bool isActive, int totalPlayerCount)
        {
            var width = 1f / totalPlayerCount;
            _playerCamera.rect = new Rect(
                width * _playerId.Value, 0, 
                width, 1f
            );
            _playerCamera.gameObject.SetActive(isActive);
        }
    }
}
