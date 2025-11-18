using System;
using InGame.Character;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "EnemyParams", menuName = "ScriptableObjects/EnemyParams")]
    public class EnemyParams : CharacterParams
    {
        public float MoveSpeed;
        public float SearchRange;
        public float AttackRange;
        public float AttackReadyTime;
        public float AttackCoolTime;
        public float DamageCoolTime;
        public Color AttackReadyColor;
        public EnemyEffectParams EffectParams;
        public AttackPatternParams[] AttackPatternParams;
    }
}
