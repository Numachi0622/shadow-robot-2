using System;
using MessagePipe;
using VContainer;
using VContainer.Unity;
using InGame.Message;
using NaughtyAttributes.Test;
using UnityEditor.SceneManagement;

namespace InGame.System
{
    public class InGameCore : ITickable, IInitializable, IDisposable
    {
        private readonly ISubscriber<StateChangeMessage> _stateChangeSubscriber;
        private readonly IPublisher<SpawnCharacterMessage> _spawnCharacterPublisher;
        private readonly IPublisher<InitGameMessage> _initGamePublisher;
        private readonly IObjectResolver _container;
        private readonly StageReferences _stageReferences;
        private readonly CharacterRegistry _characterRegistry;
        
        private StateMachine<InGameCore> _stateMachine;
        private IDisposable _stateChangeSubscription;


        public IObjectResolver Container => _container;
        public IPublisher<SpawnCharacterMessage> SpawnCharacterPublisher => _spawnCharacterPublisher;
        public StageReferences StageReferences => _stageReferences;
        public CharacterRegistry CharacterRegistry => _characterRegistry;

        [Inject]
        public InGameCore(
            IObjectResolver container,
            StageReferences stageReferences,
            CharacterRegistry characterRegistry,
            ISubscriber<StateChangeMessage> stateChangeSubscriber,
            IPublisher<SpawnCharacterMessage> spawnCharacterPublisher,
            IPublisher<InitGameMessage> initGamePublisher)
        {
            _container = container;
            _stageReferences = stageReferences;
            _characterRegistry = characterRegistry;
            _stateChangeSubscriber = stateChangeSubscriber;
            _spawnCharacterPublisher = spawnCharacterPublisher;
            _initGamePublisher = initGamePublisher;
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