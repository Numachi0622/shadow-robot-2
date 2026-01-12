using System;
using System.Text;
using Windows.Kinect;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using UnityEngine;
using NaughtyAttributes;
using OscCore;
using SynMotion;
using Utility;
using VContainer;

namespace InGame.System
{
    public class DebugCommand : MonoBehaviour
    {
        [SerializeField] private DeviceSettings _deviceSettings;
        [SerializeField] private CharacterCore _testEnemyCore;
        [SerializeField] private AttackCollider _attackCollider;
        [SerializeField] private AttackPoint _saikyoAttackParam;
        [SerializeField] private AttackPoint _saijakuAttackParam;
        [SerializeField, ReadOnly] private GameStateType currentState = GameStateType.Title;

        private IPublisher<StateChangeMessage> _stateChangePublisher;
        private IPublisher<PoseMatchEventResultMessage> _poseMatchEventResultPublisher;
        private readonly bool[] _isTestConnected = new bool[3];
        private IMotionSender _motionSender;
        private CharacterRegistry _characterRegistry;

        [Inject]
        public void Construct(
            IPublisher<StateChangeMessage> stateChangePublisher,
            IPublisher<PoseMatchEventResultMessage> poseMatchEventResultPublisher,
            CharacterRegistry characterRegistry)
        {
            _stateChangePublisher = stateChangePublisher;
            _poseMatchEventResultPublisher = poseMatchEventResultPublisher;
            _characterRegistry = characterRegistry;
        }

        private void Start()
        {
            _motionSender = new MotionSender(_deviceSettings.IpAddress, _deviceSettings.Port);
        }

        [Button]
        public void ChangeStateCommand()
        {
            var stateCount = Enum.GetValues(typeof(GameStateType)).Length;
            currentState = (GameStateType)(((int)currentState + 1) % stateCount);
            var playerCount = _characterRegistry.GetAllPlayers().Count;
            _stateChangePublisher.Publish(new StateChangeMessage(
                currentState, 
                currentState == GameStateType.NormalBattle 
                    ? new InitGameMessage(playerCount)
                    : new InitBossBattleMessage(playerCount)
                )
            );
        }

        [Button]
        public void SaikyoAttack()
        {
            var attackParam = new AttackParam()
            {
                AttackPoint = _saikyoAttackParam,
                AttackDirection = Vector3.forward,
                AttackVelocity = 0f,
                AttackType = AttackType.PlayerToEnemyNormal
            };
            _attackCollider.AttackImpactAsync(attackParam).Forget();
        }

        [Button]
        public void SaijakuAttack()
        {
            var attackParam = new AttackParam()
            {
                AttackPoint = _saijakuAttackParam,
                AttackDirection = Vector3.forward,
                AttackVelocity = 0f,
                AttackType = AttackType.PlayerToEnemyNormal
            };
            _attackCollider.AttackImpactAsync(attackParam).Forget();
        }

        [Button]
        public void TestEnemyAttack()
        {
            _testEnemyCore?.Attacker.Attack(Vector3.zero);
        }

        [Button]
        public void RegisterPlayer01()
        {
            _isTestConnected[0] = !_isTestConnected[0];
        }

        [Button]
        public void RegisterPlayer02()
        {
            _isTestConnected[1] = !_isTestConnected[1];
        }

        [Button]
        public void RegisterPlayer03()
        {
            _isTestConnected[2] = !_isTestConnected[2];
        }
        
        [Button]
        public void PoseMatchSuccess()
        {
            _poseMatchEventResultPublisher.Publish(new PoseMatchEventResultMessage(true));
        }
        
        [Button]
        public void PoseMatchFailure()
        {
            _poseMatchEventResultPublisher.Publish(new PoseMatchEventResultMessage(false));
        }

        private void Update()
        {
            for (var i = 0; i < _isTestConnected.Length; i++)
            {
                var deviceId = (i == 0 || i == 1) ? 0 : 1;
                var playerId = i % 2;
                
                if (_isTestConnected[i])
                {
                    _motionSender.SendFlag(OscAddress.GetFlagAddress(deviceId, playerId), 1);
                }
                else
                {
                    if (BodySourceManager.Instance == null) return;
                    if (BodySourceManager.Instance.Sensor == null) return;
                    if (BodySourceManager.Instance.Sensor.IsOpen) return;
                    
                    _motionSender.SendFlag(OscAddress.GetFlagAddress(deviceId, playerId), 0);
                }
            }
        }
    }
}