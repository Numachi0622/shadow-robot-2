using UnityEngine;
using VContainer.Unity;

namespace InGame.System
{
    public class TitleState : StateMachine<InGameCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[TitleState] OnEnter");
            
            Owner.StageReferences.TitleStage.SetActive(true);
        }
        
        public override void OnExit()
        {
            Owner.StageReferences.TitleStage.SetActive(false);
        }
    }
}