using System;
using System.Text;
using System.Threading;
using Windows.Kinect;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using UnityEngine;
using NaughtyAttributes;
using OscCore;
using SynMotion;
using UnityEngine.SceneManagement;
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
        [SerializeField] private AttackPoint _attackPoint;
        [SerializeField] private int _skipPlayerCount = 3;
        [SerializeField, ReadOnly] private GameStateType currentState = GameStateType.Title;

        private IPublisher<StateChangeMessage> _stateChangePublisher;
        private IPublisher<PoseMatchEventResultMessage> _poseMatchEventResultPublisher;
        private readonly bool[] _isTestConnected = new bool[3];
        private IMotionSender _motionSender;
        private CharacterRegistry _characterRegistry;
        private TextureRegistry _textureRegistry;

        [Inject]
        public void Construct(
            IPublisher<StateChangeMessage> stateChangePublisher,
            IPublisher<PoseMatchEventResultMessage> poseMatchEventResultPublisher,
            CharacterRegistry characterRegistry,
            TextureRegistry textureRegistry)
        {
            _stateChangePublisher = stateChangePublisher;
            _poseMatchEventResultPublisher = poseMatchEventResultPublisher;
            _characterRegistry = characterRegistry;
            _textureRegistry = textureRegistry;
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
                    ? new InitGameMessage(playerCount, -1)
                    : null
                )
            );
        }
        
        [Button]
        public void SkipToBossBattle()
        {
            for (var i = 0; i < _skipPlayerCount; i++)
            {
                _isTestConnected[i] = !_isTestConnected[i];
            }
            _stateChangePublisher.Publish(new StateChangeMessage(
                GameStateType.BossBattle,
                new InitGameMessage(_skipPlayerCount, -1)
            ));
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
        public void DebugAttack()
        {
            var attackParam = new AttackParam()
            {
                AttackPoint = _attackPoint,
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

        [SerializeField] private string _textureFileName = String.Empty;
        [SerializeField] private int _textureIndex = 0;
        [SerializeField, ReadOnly, ShowAssetPreview] private Texture2D _previewTexture;

        [Button]
        public async void RegisterTexture()
        {
            try
            {
                var texturePath = TexturePathSettings.GetTexturePath();
                var texture = await TextureFileLoader.LoadAsync(texturePath, _textureFileName, CancellationToken.None);
                if (texture == null) return;

                _textureRegistry.Register(_textureIndex, texture);
                _previewTexture = texture;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                                                     Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)))
            {
                ChangeStateCommand();
            }
            // Ctrl+R (Windows/Mac) または Command+R (Mac) でシーンリセット
            else if (Input.GetKeyDown(KeyCode.R) &&
                     (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                      Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)))
            {
                SceneManager.LoadScene("EntryPoint");
            }

            // Debug用のKinectが軌道していたらデバコマは使えないようにする
            if (BodySourceManager.Instance != null) return;
            
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
                    _motionSender.SendFlag(OscAddress.GetFlagAddress(deviceId, playerId), 0);
                }
            }
        }
    }
}