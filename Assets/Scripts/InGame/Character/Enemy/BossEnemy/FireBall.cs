using UnityEngine;

namespace InGame.Character
{
    public class FireBall : MonoBehaviour
    {
        [SerializeField] private AttackCollider _attackCollider;
        private FireBallAttacker _attacker;
        private AttackParam _attackParam;
        private bool _isFired;
        
        public void Fire(AttackParam attackParam)
        {
            if (_isFired) return;
            
            _attacker = new FireBallAttacker(_attackCollider);
            _attackParam = attackParam;
            _attacker.SetAttackParam(_attackParam);
            _attacker.Attack(_attackParam.AttackDirection);
            _isFired = true;
            
            // 5秒後に自動で消えるようにする
            Destroy(gameObject, 5f);
        }

        private void Update()
        {
            if (!_isFired) return;
            transform.position += _attackParam.AttackDirection.normalized * (_attackParam.AttackVelocity * Time.deltaTime);
        }
    }
}