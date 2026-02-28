using UnityEngine;

namespace InGame.Character
{
    public class PlayerAttacker : Attacker
    {
        private readonly CharacterCore _owner;
        private readonly RocketPunchAttackPattern _rocketPunchAttackPattern;
        private readonly float _rocketPunchCooldown;
        private readonly PlayerParams _playerParams;
        private float _lastRocketPunchTime = -1f;

        public PlayerAttacker(
            PlayerParams playerParams,
            AttackCollider attackCollider,
            CharacterCore characterCore = null,
            RocketPunchAttackPattern rocketPunchAttackPattern = null,
            float rocketPunchCooldown = 0.5f) : base(playerParams, attackCollider)
        {
            _owner = characterCore;
            _rocketPunchAttackPattern = rocketPunchAttackPattern;
            _rocketPunchCooldown = rocketPunchCooldown;
            _playerParams = playerParams;
        }

        private void SetAttackParam(Vector3 dir, float velocity, AttackType attackType)
        {
            _attackParam = new AttackParam()
            {
                AttackPoint = _attackPoint,
                AttackDirection = dir,
                AttackVelocity = velocity,
                AttackType = attackType,
                Origin = _attackCollider.transform.position,
                AttackerId = _owner != null ? _owner.Id : default
            };
        }

        public override void Attack(Vector3 dir, float velocity = 0f, float waitTime = 0f)
        {
            SetAttackParam(dir, velocity, AttackType.PlayerToEnemyNormal);

            _attackCollider.AttackImpact(_attackParam);

            // RocketPunchの発射条件チェック
            if (_rocketPunchAttackPattern != null && CanFireRocketPunch(dir, velocity))
            {
                _rocketPunchAttackPattern.Execute(_owner, _attackParam);
                _lastRocketPunchTime = Time.time;
            }
        }

        private bool CanFireRocketPunch(Vector3 dir, float velocity)
        {
            if (dir.z < 0f || dir.y < -0.3f) return false;

            // クールダウン時間が経過しているかチェック
            if (Time.time - _lastRocketPunchTime < _rocketPunchCooldown) return false;

            if (velocity < _playerParams.RocketPunchVelocityThreshold) return false;
            Debug.Log(velocity);

            return true;
        }

        public override void AttackEnd()
        {
            _attackCollider.AttackImpactEnd();
        }
    }
}
