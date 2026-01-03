using SynMotion;
using UnityEngine;

namespace InGame.Character
{
    public class CharacterCore : MonoBehaviour
    {
        protected IAttackable _attacker;
        protected IDamageable _damager;
        protected IMovable _mover;
        
        public IAttackable Attacker => _attacker;
        public IDamageable Damager => _damager;
        public IMovable Mover => _mover;

        public virtual void Initialize(CharacterId id, SynMotionSystem synMotion, int totalPlayerCount = -1)
        {
        }

        public virtual void Initialize()
        {
        }

        public virtual void OnUpdate()
        {
        }
    }
}
