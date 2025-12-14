using System;
using System.Collections.Generic;
using InGame.Character;
using InGame.Environment;
using MessagePipe;
using VContainer;
using VContainer.Unity;
using InGame.Message;

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
        public IPublisher<AllEnemyDespawnMessage> AllEnemyDespawnMessage { get; private set; }
        public IPublisher<AllPlayerDespawnMessage> AllPlayerDespawnMessage { get; private set; }
        public StageReferences StageReferences { get; private set; }
        public CharacterRegistry CharacterRegistry { get; private set; }
        public MainStageManager MainStageManager => _mainStageManager;
        public List<BuildingCore> BuildingPrefabs { get; private set; }

        [Inject]
        public InGameCore(
            IObjectResolver container,
            StageReferences stageReferences,
            CharacterRegistry characterRegistry,
            MainStageManager mainStageManager,
            CharacterPrefabs characterPrefabs,
            ISubscriber<StateChangeMessage> stateChangeSubscriber,
            IPublisher<SpawnCharacterMessage> spawnCharacterPublisher,
            IPublisher<CreateBuildingMessage> createBuildingPublisher,
            IPublisher<InitGameMessage> initGamePublisher,
            IPublisher<CharacterId, GameStartPlayerInitMessage> gameStartPlayerInitPublisher,
            IPublisher<AllEnemyDespawnMessage> allEnemyDespawnMessage,
            IPublisher<AllPlayerDespawnMessage> allPlayerDespawnMessage)
        {
            Container = container;
            StageReferences = stageReferences;
            CharacterRegistry = characterRegistry;
            _mainStageManager = mainStageManager;
            BuildingPrefabs = characterPrefabs.BuildingPrefabs;
            _stateChangeSubscriber = stateChangeSubscriber;
            SpawnCharacterPublisher = spawnCharacterPublisher;
            CreateBuildingPublisher = createBuildingPublisher;
            InitGamePublisher = initGamePublisher;
            GameStartPlayerInitPublisher = gameStartPlayerInitPublisher;
            AllEnemyDespawnMessage = allEnemyDespawnMessage;
            AllPlayerDespawnMessage = allPlayerDespawnMessage;
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