namespace InGame.System
{
    public class GameOverState : StateMachine<InGameCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            UnityEngine.Debug.Log("[GameOverState] OnEnter");
        }
    }
}