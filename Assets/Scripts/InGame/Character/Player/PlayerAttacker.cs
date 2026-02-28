using UnityEngine;

namespace InGame.Character
{
    public class PlayerAttacker : Attacker
    {
        private readonly CharacterCore _owner;
        private readonly RocketPunchAttackPattern _rocketPunchAttackPattern;
        private readonly float _rocketPunchCooldown;
        private float _lastRocketPunchTime = -1f;

        public PlayerAttacker(
            CharacterParams characterParams,
            AttackCollider attackCollider,
            CharacterCore characterCore = null,
            RocketPunchAttackPattern rocketPunchAttackPattern = null,
            float rocketPunchCooldown = 0.5f) : base(characterParams, attackCollider)
        {
            _owner = characterCore;
            _rocketPunchAttackPattern = rocketPunchAttackPattern;
            _rocketPunchCooldown = rocketPunchCooldown;
        }

        private void SetAttackParam(Vector3 dir, float velocity, AttackType attackType)
        {
            _attackParam = new AttackParam()
            {
                AttackPoint = _attackPoint,
                AttackDirection = dir,
                AttackVelocity = velocity,
                AttackType = attackType
            };
        }

        public override void Attack(Vector3 dir, float velocity = 0f, float waitTime = 0f)
        {
            SetAttackParam(dir, velocity, AttackType.PlayerToEnemyNormal);

            _attackCollider.AttackImpact(_attackParam);

            // RocketPunchの発射条件チェック
            if (_rocketPunchAttackPattern != null && CanFireRocketPunch(dir))
            {
                _rocketPunchAttackPattern.Execute(_owner, _attackParam);
                _lastRocketPunchTime = Time.time;
            }
        }

        private bool CanFireRocketPunch(Vector3 dir)
        {
            // 入力方向のz成分が0以上かチェック
            if (dir.z < 0f) return false;

            // クールダウン時間が経過しているかチェック
            if (Time.time - _lastRocketPunchTime < _rocketPunchCooldown) return false;

            return true;
        }

        public override void AttackEnd()
        {
            _attackCollider.AttackImpactEnd();
        }
    }
}
