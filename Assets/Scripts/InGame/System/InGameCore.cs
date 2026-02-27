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
    public class InGameContext
    {
        public int PlayerCount { get; private set; }
        public InGameContext(int playerCount)
        {
            PlayerCount = playerCount;
        }
    }
    
    public class InGameCore : ITickable, IInitializable, IDisposable
    {
        private readonly ISubscriber<StateChangeMessage> _stateChangeSubscriber;
        private readonly MainStageManager _mainStageManager;
        
        private StateMachine<InGameCore> _stateMachine;
        private InGameContext _context;
        private IDisposable _stateChangeSubscription;
        
        public InGameContext Context => _context;
        public IObjectResolver Container { get; private set; }
        public IPublisher<SpawnCharacterMessage> SpawnCharacterPublisher { get; private set; }
        public IPublisher<DespawnCharacterMessage> DespawnCharacterPublisher { get; private set; }
        public IPublisher<CreateBuildingMessage> CreateBuildingPublisher { get; private set; }
        public IPublisher<InitGameMessage> InitGamePublisher { get; private set; }
        public IPublisher<CharacterId, GameStartPlayerInitMessage> GameStartPlayerInitPublisher { get; private set; }
        public IPublisher<NormalBattleEndMessage> NormalBattleEndPublisher { get; private set; }
        public IPublisher<AllEnemyDespawnMessage> AllEnemyDespawnMessage { get; private set; }
        public IPublisher<AllEnemyStopMessage> AllEnemyStopMessage { get; private set; }
        public IPublisher<AllPlayerDespawnMessage> AllPlayerDespawnMessage { get; private set; }
        public IPublisher<CombineCompleteMessage> CombineCompletePublisher { get; private set; }
        public IPublisher<BossBattleStartMessage> BossBattleStartPublisher { get; private set; }
        public ISubscriber<CharacterSpawnRequestMessage> CharacterSpawnRequestSubscriber { get; private set; }
        public ISubscriber<CharacterDespawnRequestMessage> CharacterDespawnRequestSubscriber { get; private set; }
        public ISubscriber<ConnectionRecoverMessage> ConnectionRecoverSubscriber { get; private set; }
        public ISubscriber<PoseMatchEventStartMessage> PoseMatchEventStartSubscriber { get; private set; }
        public ISubscriber<PoseMatchEventResultMessage> PoseMatchEventResultSubscriber { get; private set; }
        public IPublisher<PoseMatchEventEndMessage> PoseMatchEventEndPublisher { get; private set; }
        public IPublisher<OpenShieldMessage> OpenShieldPublisher { get; private set; }
        public StageReferences StageReferences { get; private set; }
        public CharacterRegistry CharacterRegistry { get; private set; }
        public MainStageManager MainStageManager => _mainStageManager;
        public List<BuildingCore> BuildingPrefabs { get; private set; }
        public InGameUIController InGameUIController { get; set; }
        public PlayableDirectorReferences PlayableDirectorReferences { get; set; }
        public TextureRegistry TextureRegistry { get; private set; }

        [Inject]
        public InGameCore(
            IObjectResolver container,
            StageReferences stageReferences,
            CharacterRegistry characterRegistry,
            MainStageManager mainStageManager,
            CharacterPrefabs characterPrefabs,
            InGameUIController inGameUIController,
            PlayableDirectorReferences playableDirectorReferences,
            ISubscriber<StateChangeMessage> stateChangeSubscriber,
            IPublisher<SpawnCharacterMessage> spawnCharacterPublisher,
            IPublisher<DespawnCharacterMessage> despawnCharacterPublisher,
            IPublisher<CreateBuildingMessage> createBuildingPublisher,
            IPublisher<InitGameMessage> initGamePublisher,
            IPublisher<CharacterId, GameStartPlayerInitMessage> gameStartPlayerInitPublisher,
            IPublisher<NormalBattleEndMessage> normalBattleEndPublisher,
            IPublisher<AllEnemyDespawnMessage> allEnemyDespawnMessage,
            IPublisher<AllEnemyStopMessage> allEnemyStopMessage,
            IPublisher<AllPlayerDespawnMessage> allPlayerDespawnMessage,
            IPublisher<CombineCompleteMessage> combineCompletePublisher,
            IPublisher<BossBattleStartMessage> bossBattleStartPublisher,
            ISubscriber<CharacterSpawnRequestMessage> characterSpawnRequestSubscriber,
            ISubscriber<CharacterDespawnRequestMessage> characterDespawnRequestSubscriber,
            ISubscriber<ConnectionRecoverMessage> connectionRecoverSubscriber,
            ISubscriber<PoseMatchEventStartMessage> poseMatchEventStartSubscriber,
            ISubscriber<PoseMatchEventResultMessage> poseMatchEventResultSubscriber,
            IPublisher<PoseMatchEventEndMessage> poseMatchEventEndPublisher,
            IPublisher<OpenShieldMessage> openShieldPublisher,
            TextureRegistry textureRegistry)
        {
            Container = container;
            StageReferences = stageReferences;
            CharacterRegistry = characterRegistry;
            _mainStageManager = mainStageManager;
            BuildingPrefabs = characterPrefabs.BuildingPrefabs;
            InGameUIController = inGameUIController;
            PlayableDirectorReferences = playableDirectorReferences;
            _stateChangeSubscriber = stateChangeSubscriber;
            SpawnCharacterPublisher = spawnCharacterPublisher;
            DespawnCharacterPublisher = despawnCharacterPublisher;
            CreateBuildingPublisher = createBuildingPublisher;
            InitGamePublisher = initGamePublisher;
            GameStartPlayerInitPublisher = gameStartPlayerInitPublisher;
            NormalBattleEndPublisher = normalBattleEndPublisher;
            AllEnemyDespawnMessage = allEnemyDespawnMessage;
            AllEnemyStopMessage = allEnemyStopMessage;
            AllPlayerDespawnMessage = allPlayerDespawnMessage;
            CombineCompletePublisher = combineCompletePublisher;
            BossBattleStartPublisher = bossBattleStartPublisher;
            CharacterSpawnRequestSubscriber = characterSpawnRequestSubscriber;
            CharacterDespawnRequestSubscriber = characterDespawnRequestSubscriber;
            ConnectionRecoverSubscriber = connectionRecoverSubscriber;
            PoseMatchEventStartSubscriber = poseMatchEventStartSubscriber;
            PoseMatchEventResultSubscriber = poseMatchEventResultSubscriber;
            PoseMatchEventEndPublisher = poseMatchEventEndPublisher;
            OpenShieldPublisher = openShieldPublisher;
            TextureRegistry = textureRegistry;
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
        
        public void SetContext(InGameContext context)
        {
            if (_context != null) return;
            _context = context;
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