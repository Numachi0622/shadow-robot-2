using InGame.Message;

namespace InGame.System
{
    public class GameOverState : StateMachine<InGameCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            UnityEngine.Debug.Log("[GameOverState] OnEnter");
            Owner.AllEnemyStopMessage.Publish(new AllEnemyStopMessage());
            Owner.InGameUIController.ShowGameOverView();
        }
    }
}