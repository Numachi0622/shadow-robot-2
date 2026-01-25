using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "AttackPattern", menuName = "Character/AttackPattern/Default", order = 1)]
    public class AttackPattern : ScriptableObject, IAttackPattern
    {
        public virtual void ExecuteReady(CharacterCore owner)
        {
        }
        public virtual void Execute(CharacterCore owner, AttackReadyParam attackParam)
        {
        }
    }
}