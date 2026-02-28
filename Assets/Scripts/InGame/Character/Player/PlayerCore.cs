using System;
using Cysharp.Threading.Tasks;
using InGame.Message;
using InGame.System;
using InGame.System.UI;
using MessagePipe;
using SynMotion;
using UniRx;
using UnityEngine;
using Utility;
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
        [SerializeField] private PlayerShield _shield;
        [SerializeField] private Transform _center;
        [SerializeField] private Renderer _bodyRenderer;
        [SerializeField] private Renderer _leftArmRenderer;
        [SerializeField] private Renderer _rightArmRenderer;
        [SerializeField] private Renderer _footPartsRenderer;
        [SerializeField] private RocketPunchAttackPattern _rocketPunchPattern;

        private CharacterRegistry _characterRegistry;
        private HitPointView _hpView;
        private StateMachine<PlayerCore> _stateMachine;
        private IAttackable _leftHandAttacker, _rightHandAttacker;
        private HandAttackObserver _leftHandObserver, _rightHandObserver;
        private DamageObserver _damageObserver;
        private PlayerMotionMover _motionMover;
        private SynMotionSystem _synMotion;
        private CharacterId _playerId;
        private CharacterId _footPlayerId;
        private JumpCalibrationSystem _calibrationSystem;
        private bool _isMovable = false;
        private bool _isCalibrated = false;
        private Vector3 _baseWorldPosition;
        private Vector3 _baseTrackingPosition;
        private float _calibratedGroundHeight;
        private bool _isJumping;
        private bool _isCombine;
        private int _totalPlayerCount = -1;
        private readonly float _jumpThresholdOffset = 0.05f;
        private readonly float _jumpWeight = 6f;
        private Material _material1;
        private Material _material2;
        private Material _leftArmMaterial;
        private Material _rightArmMaterial;
        private Material _footPartsMaterial;
        
        private ISubscriber<CharacterId, GameStartPlayerInitMessage> _gameStartPlayerInitSubscriber;
        private ISubscriber<AllPlayerDespawnMessage> _allPlayerDespawnSubscriber;
        private IPublisher<DespawnCharacterMessage> _despawnPublisher;
        private ISubscriber<CombineCompleteMessage> _combineCompleteSubscriber;
        private ISubscriber<BossBattleStartMessage> _bossBattleStartSubscriber;
        private ISubscriber<OpenShieldMessage> _openShieldSubscriber;
        private IPublisher<StateChangeMessage> _stateChangePublisher;
        
        public CharacterId PlayerId => _playerId;
        
        public IAttackable LeftHandAttacker => _leftHandAttacker;
        public IAttackable RightHandAttacker => _rightHandAttacker;
        public IDamageable Damager => _damager;
        public MovementTransforms Transforms => _movementTransforms;
        public Camera PlayerCamera => _playerCamera;
        public HitPointPresenter HpPresenter => _hpPresenter;
        public Transform Center => _center;
        public Transform GetNearestEnemy(Vector3 pos) => _characterRegistry.GetNearestEnemy(pos)?.Center;

        public override void Initialize(CharacterId id, SynMotionSystem synMotion, int totalPlayerCount = -1)
        {
            _damager = new Damager(_hpPresenter);
            
            _playerId = id;
            _synMotion = synMotion;
            _totalPlayerCount = totalPlayerCount;
            _isCombine = _totalPlayerCount > 1;
            _footPlayerId = _isCombine ? new CharacterId(totalPlayerCount - 1) : id;
            
            _stateMachine = new StateMachine<PlayerCore>(this);
            _leftHandObserver = new HandAttackObserver(_movementTransforms.LeftHand, _params.AttackableVelocity);
            _rightHandObserver = new HandAttackObserver(_movementTransforms.RightHand, _params.AttackableVelocity);
            _damageObserver = new DamageObserver();
            _leftHandAttacker = !_isCombine
                ? new PlayerAttacker(_params, _leftHandCollider)
                : new PlayerAttacker(_params, _leftHandCollider, this, _rocketPunchPattern);
            _rightHandAttacker = !_isCombine
                ? new PlayerAttacker(_params, _leftHandCollider)
                : new PlayerAttacker(_params, _leftHandCollider, this, _rocketPunchPattern);
            _mover = new PlayerMover(_movementTransforms.Reference);
            _motionMover = new PlayerMotionMover(_movementTransforms);
            
            _damageCollider.Initialize(_damageObserver);
            _hpPresenter.Initialize(_hpView, _params);
            gameObject.name += _playerId.Value.ToString();

            Bind();

            if (_isCombine)
            {
                _baseTrackingPosition = Vector3.zero;
                transform.position = _baseWorldPosition;
            }

            // キャリブレーション開始
            _calibrationSystem = new JumpCalibrationSystem(_synMotion, _footPlayerId);
            CalibrateAsync().Forget();

            _material2 = _bodyRenderer.materials[0];
            _material1 = _bodyRenderer.materials[1];
            if (_leftArmRenderer) _leftArmMaterial = _leftArmRenderer.material;
            if (_rightArmRenderer) _rightArmMaterial = _rightArmRenderer.material;
            if (_footPartsRenderer) _footPartsMaterial = _footPartsRenderer.material;
        }

        public void SetTexture(ITextureContext context)
        {
            if (_material1 == null || _material2 == null)
            {
                Debug.LogError("[PlayerCore] Materials are not initialized.");
                return;
            }

            _material1.SetTexture(GameConst.ShaderMainTexture1, context.Texture1);
            _material1.EnableKeyword(GameConst.ShaderKeywordMainTexture1On);
            _material2.SetTexture(GameConst.ShaderMainTexture2, context.Texture2);
            _material2.EnableKeyword(GameConst.ShaderKeywordMainTexture2On);

            // ここからは合体用のテクスチャ割り当て
            if (!_leftArmMaterial || !_rightArmMaterial || !_footPartsMaterial) return;
            if (context.LeftArmTexture == null) return;

            _material1.SetTexture(GameConst.ShaderLeftHandTexture, context.LeftArmTexture);
            _material1.EnableKeyword(GameConst.ShaderKeywordLeftHandTextureOn);
            _material1.SetTexture(GameConst.ShaderRightHandTexture, context.RightArmTexture);
            _material1.EnableKeyword(GameConst.ShaderKeywordRightHandTextureOn);
            _material2.SetTexture(GameConst.ShaderFootTexture, context.FootPartsTexture);
            _material2.EnableKeyword(GameConst.ShaderKeywordFootTextureOn);

            _leftArmMaterial.SetTexture(GameConst.ShaderMainTexture1, context.Texture1);
            _leftArmMaterial.EnableKeyword(GameConst.ShaderKeywordMainTexture1On);
            _rightArmMaterial.SetTexture(GameConst.ShaderMainTexture1, context.Texture1);
            _rightArmMaterial.EnableKeyword(GameConst.ShaderKeywordMainTexture1On);
            _footPartsMaterial.SetTexture(GameConst.ShaderMainTexture2, context.Texture2);
            _footPartsMaterial.EnableKeyword(GameConst.ShaderKeywordMainTexture2On);
        }

        private async UniTaskVoid CalibrateAsync()
        {
            _calibratedGroundHeight = await _calibrationSystem.CalibrateGroundHeight(60);
            _isCalibrated = true;
        }

        [Inject]
        public void Construct(
            HitPointViewList hpViewList,
            CharacterRegistry characterRegistry,
            ISubscriber<CharacterId, GameStartPlayerInitMessage> gameStartPlayerInitSubscriber,
            ISubscriber<AllPlayerDespawnMessage> allPlayerDespawnSubscriber,
            IPublisher<DespawnCharacterMessage> despawnPublisher,
            ISubscriber<CombineCompleteMessage> combineCompleteSubscriber,
            ISubscriber<BossBattleStartMessage> bossBattleStartSubscriber,
            ISubscriber<OpenShieldMessage> openShieldSubscriber,
            IPublisher<StateChangeMessage> stateChangePublisher)
        {
            _hpView = hpViewList.PlayerHitPointView;
            _characterRegistry = characterRegistry;
            _gameStartPlayerInitSubscriber = gameStartPlayerInitSubscriber;
            _allPlayerDespawnSubscriber = allPlayerDespawnSubscriber;
            _despawnPublisher = despawnPublisher;
            _combineCompleteSubscriber = combineCompleteSubscriber;
            _bossBattleStartSubscriber = bossBattleStartSubscriber;
            _openShieldSubscriber = openShieldSubscriber;
            _stateChangePublisher = stateChangePublisher;
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
                .Subscribe(OnRightHandAttackEnd)
                .AddTo(this);

            _damageObserver.OnTakeDamage
                .Subscribe(OnDamageStart)
                .AddTo(this);

            _hpPresenter.OnHpDecreased
                .Subscribe(OnDeadStart)
                .AddTo(this);
            
            // MessagePipeを購読
            _gameStartPlayerInitSubscriber?.Subscribe(_playerId, OnGameStartInitialize).AddTo(this);
            _allPlayerDespawnSubscriber?.Subscribe(_ =>
            {
                _despawnPublisher.Publish(new DespawnCharacterMessage(_playerId));
                Destroy(gameObject);
            }).AddTo(this);
            _combineCompleteSubscriber?.Subscribe(_ => OnCombineComplete()).AddTo(this);
            _bossBattleStartSubscriber?.Subscribe(_ => OnBossBattleStart()).AddTo(this);
            _openShieldSubscriber?.Subscribe(_ => OpenShield()).AddTo(this);
        }

        public override void OnUpdate()
        {
            _leftHandObserver.Observe();
            _rightHandObserver.Observe();

            var motionParam = _isCombine
                ? _synMotion.GetCombineMotionParam(_totalPlayerCount)
                : _synMotion.GetMotionParam(_playerId.Value);
            _motionMover.UpdateMotion(motionParam);

            if (!_isMovable || !_isCalibrated) return; 
            var moveValue = motionParam.SpineMidPosition * _params.MoveWeight - _baseTrackingPosition;
            moveValue.y = motionParam.SpineMidPosition.y;
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
        
        private void OnRightHandAttackEnd(Unit unit)
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
            _stateChangePublisher.Publish(new StateChangeMessage(GameStateType.GameOver));
        }
        #endregion

        #region MessagePipe Event

        private void OnGameStartInitialize(GameStartPlayerInitMessage message)
        {
            _baseWorldPosition = message.GameStartPosition;
            transform.position = _baseWorldPosition;

            // トラッキング位置を基準位置に設定
            var trackingPos = _synMotion.GetMotionParam(_footPlayerId.Value).SpineMidPosition * _params.MoveWeight;
            _baseTrackingPosition = trackingPos;
            
            SetMovable(true);
            SetCamera(true, message.TotalPlayerCount);
        }
        
        private void OnCombineComplete()
        {
            _playerCamera.gameObject.SetActive(true);
        }

        private void OnBossBattleStart()
        {
            _isMovable = true;

            // 合体時は現在のトラッキング位置を基準位置に設定
            if (_isCombine)
            {
                var trackingPos = _synMotion.GetCombineMotionParam(_totalPlayerCount).SpineMidPosition * _params.MoveWeight;
                _baseTrackingPosition = trackingPos;
                transform.position = _baseWorldPosition;
            }
        }

        private void OpenShield()
        {
            _shield.OpenShieldAsync().Forget();
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
