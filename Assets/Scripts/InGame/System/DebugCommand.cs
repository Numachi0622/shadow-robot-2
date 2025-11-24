using System;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using UnityEngine;
using NaughtyAttributes;
using Utility;
using VContainer;

namespace InGame.System
{
    public class DebugCommand : MonoBehaviour
    {
        [SerializeField] private CharacterCore _testEnemyCore;
        [SerializeField] private AttackCollider _attackCollider;
        [SerializeField] private AttackPoint _saikyoAttackParam;
        [SerializeField] private AttackPoint _saijakuAttackParam;
        [ReadOnly] [SerializeField] private GameStateType currentState = GameStateType.Title;
        
        private IPublisher<StateChangeMessage> _stateChangePublisher;
        
        [Inject]
        public void Construct(IPublisher<StateChangeMessage> stateChangePublisher)
        {
            _stateChangePublisher = stateChangePublisher;
        }

        [Button]
        public void ChangeStateCommand()
        {
            var stateCount = Enum.GetValues(typeof(GameStateType)).Length;
            currentState = (GameStateType)(((int)currentState + 1) % stateCount);
            _stateChangePublisher.Publish(new StateChangeMessage(currentState));
        }

        [Button]
        public void SaikyoAttack()
        {
            var attackParam = new AttackParam()
            {
                AttackPoint = _saikyoAttackParam,
                AttackDirection = Vector3.forward,
                AttackVelocity = 0f,
                AttackType = AttackType.PlayerToEnemyNormal
            };
            _attackCollider.AttackImpactAsync(attackParam).Forget();
        }
        
        [Button]
        public void SaijakuAttack()
        {
            var attackParam = new AttackParam()
            {
                AttackPoint = _saijakuAttackParam,
                AttackDirection = Vector3.forward,
                AttackVelocity = 0f,
                AttackType = AttackType.PlayerToEnemyNormal
            };
            _attackCollider.AttackImpactAsync(attackParam).Forget();
        }
        
        [Button]
        public void TestEnemyAttack()
        {
            _testEnemyCore?.Attacker.Attack(Vector3.zero);
        }
    }
}