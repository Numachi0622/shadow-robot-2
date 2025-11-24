using UnityEngine;

namespace InGame.System
{
    public class TitleState : StateMachine<InGameCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[TitleState] OnEnter");
        }
    }
}