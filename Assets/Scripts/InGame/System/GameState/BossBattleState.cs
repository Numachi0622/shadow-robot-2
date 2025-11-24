using UnityEngine;

namespace InGame.System
{
    public class BossBattleState : StateMachine<InGameCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[BossBattleState] OnEnter");
        }
    }
}
