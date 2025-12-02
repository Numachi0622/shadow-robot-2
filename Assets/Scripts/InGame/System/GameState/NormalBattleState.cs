using InGame.Message;
using UnityEngine;
using VContainer.Unity;

namespace InGame.System
{
    public class NormalBattleState : StateMachine<InGameCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalBattleState] OnEnter");
            Owner.SpawnCharacterPublisher.Publish(new SpawnCharacterMessage(
                CharacterType.NormalEnemy,
                new Vector3(0, 0, 0),
                Quaternion.identity
            ));
            
            Owner.StageReferences.MainStage.SetActive(false);
        }
    }
}
