using UnityEngine;

namespace InGame.System
{
    public class ResultState : StateMachine<InGameCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[ResultState] OnEnter");
        }
    }
}
