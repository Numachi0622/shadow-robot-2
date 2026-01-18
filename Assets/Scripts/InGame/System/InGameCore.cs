using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using InGame.Character;
using InGame.Environment;
using MessagePipe;
using VContainer;
using VContainer.Unity;
using InGame.Message;
using InGame.System.UI;

namespace InGame.System
{
    public class InGameCore : ITickable, IInitializable, IDisposable
    {
        private readonly ISubscriber<StateChangeMessage> _stateChangeSubscriber;
        private readonly MainStageManager _mainStageManager;
        
        private StateMachine<InGameCore> _stateMachine;
        private IDisposable _stateChangeSubscription;
        
        public IObjectResolver Container { get; private set; }
        public IPublisher<SpawnCharacterMessage> SpawnCharacterPublisher { get; private set; }
        public IPublisher<CreateBuildingMessage> CreateBuildingPublisher { get; private set; }
        public IPublisher<InitGameMessage> InitGamePublisher { get; private set; }
        public IPublisher<CharacterId, GameStartPlayerInitMessage> GameStartPlayerInitPublisher { get; private set; }
        public IPublisher<NormalBattleEndMessage> NormalBattleEndPublisher { get; private set; }
        public IPublisher<AllEnemyDespawnMessage> AllEnemyDespawnMessage { get; private set; }
        public IPublisher<AllPlayerDespawnMessage> AllPlayerDespawnMessage { get; private set; }
        public IPublisher<BossBattleStartMessage> BossBattleStartPublisher { get; private set; }
        public ISubscriber<PoseMatchEventStartMessage> PoseMatchEventStartSubscriber { get; private set; }
        public ISubscriber<PoseMatchEventResultMessage> PoseMatchEventResultSubscriber { get; private set; }
        public IPublisher<PoseMatchEventEndMessage> PoseMatchEventEndPublisher { get; private set; }
        public IPublisher<OpenShieldMessage> OpenShieldPublisher { get; private set; }
        public StageReferences StageReferences { get; private set; }
        public CharacterRegistry CharacterRegistry { get; private set; }
        public MainStageManager MainStageManager => _mainStageManager;
        public List<BuildingCore> BuildingPrefabs { get; private set; }
        public InGameUIController InGameUIController { get; set; }

        [Inject]
        public InGameCore(
            IObjectResolver container,
            StageReferences stageReferences,
            CharacterRegistry characterRegistry,
            MainStageManager mainStageManager,
            CharacterPrefabs characterPrefabs,
            InGameUIController inGameUIController,
            ISubscriber<StateChangeMessage> stateChangeSubscriber,
            IPublisher<SpawnCharacterMessage> spawnCharacterPublisher,
            IPublisher<CreateBuildingMessage> createBuildingPublisher,
            IPublisher<InitGameMessage> initGamePublisher,
            IPublisher<CharacterId, GameStartPlayerInitMessage> gameStartPlayerInitPublisher,
            IPublisher<NormalBattleEndMessage> normalBattleEndPublisher,
            IPublisher<AllEnemyDespawnMessage> allEnemyDespawnMessage,
            IPublisher<AllPlayerDespawnMessage> allPlayerDespawnMessage,
            IPublisher<BossBattleStartMessage> bossBattleStartPublisher,
            ISubscriber<PoseMatchEventStartMessage> poseMatchEventStartSubscriber,
            ISubscriber<PoseMatchEventResultMessage> poseMatchEventResultSubscriber,
            IPublisher<PoseMatchEventEndMessage> poseMatchEventEndPublisher,
            IPublisher<OpenShieldMessage> openShieldPublisher)
        {
            Container = container;
            StageReferences = stageReferences;
            CharacterRegistry = characterRegistry;
            _mainStageManager = mainStageManager;
            BuildingPrefabs = characterPrefabs.BuildingPrefabs;
            InGameUIController = inGameUIController;
            _stateChangeSubscriber = stateChangeSubscriber;
            SpawnCharacterPublisher = spawnCharacterPublisher;
            CreateBuildingPublisher = createBuildingPublisher;
            InitGamePublisher = initGamePublisher;
            GameStartPlayerInitPublisher = gameStartPlayerInitPublisher;
            NormalBattleEndPublisher = normalBattleEndPublisher;
            AllEnemyDespawnMessage = allEnemyDespawnMessage;
            AllPlayerDespawnMessage = allPlayerDespawnMessage;
            BossBattleStartPublisher = bossBattleStartPublisher;
            PoseMatchEventStartSubscriber = poseMatchEventStartSubscriber;
            PoseMatchEventResultSubscriber = poseMatchEventResultSubscriber;
            PoseMatchEventEndPublisher = poseMatchEventEndPublisher;
            OpenShieldPublisher = openShieldPublisher;
        }

        private void Bind()
        {
            _stateChangeSubscription = _stateChangeSubscriber.Subscribe(OnStateChange);
        }
        
        public void Initialize()
        {
            Bind();
            
            _stateMachine = new StateMachine<InGameCore>(this);
            _stateMachine.SetState<TitleState>();
            _mainStageManager.Initialize();
            InGameUIController.Initialize();
        }
        
        public void Tick()
        {
            _stateMachine.OnUpdate();
        }

        #region State Events

        private void OnStateChange(StateChangeMessage message)
        {
            switch (message.StateType)
            {
                case GameStateType.Title: 
                    OnTitleStart(message);
                    break;
                case GameStateType.NormalBattle:
                    OnNormalBattleStart(message);
                    break;
                case GameStateType.BossBattle:
                    OnBossBattleStart(message);
                    break;
                case GameStateType.GameOver:
                    OnGameOverStart(message);
                    break;
                case GameStateType.Result:
                    OnResultStart(message);
                    break;
            }
        }
        private void OnTitleStart(StateChangeMessage message)
        {
            _stateMachine.SetState<TitleState>(message.Parameter);
        }
        private void OnNormalBattleStart(StateChangeMessage message)
        {
            _stateMachine.SetState<NormalBattleState>(message.Parameter);
        }
        private void OnBossBattleStart(StateChangeMessage message)
        {
            _stateMachine.SetState<BossBattleState>(message.Parameter);
        }
        private void OnGameOverStart(StateChangeMessage message)
        {
            _stateMachine.SetState<GameOverState>(message.Parameter);
        }
        private void OnResultStart(StateChangeMessage message)
        {
            _stateMachine.SetState<ResultState>(message.Parameter);
        }
        #endregion

        public void Dispose()
        {
            _stateChangeSubscription?.Dispose();
        }
    }
}