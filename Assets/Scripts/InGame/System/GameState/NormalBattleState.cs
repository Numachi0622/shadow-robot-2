using UnityEngine;

namespace InGame.System
{
    public class NormalBattleState : StateMachine<InGameCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalBattleState] OnEnter");
        }
    }
}
