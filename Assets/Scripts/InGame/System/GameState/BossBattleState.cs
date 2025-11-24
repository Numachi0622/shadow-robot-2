using UnityEngine;

namespace InGame.System
{
    public class BossBattleState : StateMachine<InGameCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[BossBattleState] OnEnter");
            Owner.SpawnCharacterPublisher.Publish(new Message.SpawnCharacterMessage(
                Message.CharacterType.BossEnemy,
                new Vector3(0, 0, 0),
                Quaternion.identity
            ));
        }
    }
}
