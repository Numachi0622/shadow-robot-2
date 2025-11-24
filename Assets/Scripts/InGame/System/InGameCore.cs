using System;
using MessagePipe;
using VContainer;
using VContainer.Unity;
using InGame.Message;

namespace InGame.System
{
    public class InGameCore : ITickable, IInitializable, IDisposable
    {
        private ISubscriber<StateChangeMessage> _stateChangeSubscriber;
        private IDisposable _stateChangeSubscription;

        private StateMachine<InGameCore> _stateMachine;

        [Inject]
        public void Construct(ISubscriber<StateChangeMessage> stateChangeSubscriber)
        {
            _stateChangeSubscriber = stateChangeSubscriber;

            Bind();
        }

        private void Bind()
        {
            _stateChangeSubscription = _stateChangeSubscriber.Subscribe(OnStateChange);
        }
        
        public void Initialize()
        {
            _stateMachine = new StateMachine<InGameCore>(this);
            _stateMachine.SetState<NormalBattleState>();
        }
        
        public void Tick()
        {
            _stateMachine.OnUpdate();
        }

        #region State Events
        public void OnStateChange(StateChangeMessage message)
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