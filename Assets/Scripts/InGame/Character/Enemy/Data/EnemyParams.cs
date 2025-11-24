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

        /// <summary>
        /// OccurrenceRateを考慮した重み付けランダム選択で攻撃パターンを選ぶ
        /// </summary>
        /// <returns>選択されたパターンのインデックス</returns>
        public int SelectAttackPatternIndex()
        {
            if (AttackPatternParams == null || AttackPatternParams.Length == 0) return 0;

            // OccurrenceRateの合計を計算
            float totalRate = 0f;
            foreach (var pattern in AttackPatternParams)
            {
                totalRate += pattern.OccurrenceRate;
            }

            // 合計が0の場合は均等選択
            if (totalRate <= 0f)
            {
                return UnityEngine.Random.Range(0, AttackPatternParams.Length);
            }

            // 重み付けランダム選択
            float randomPoint = UnityEngine.Random.Range(0f, totalRate);
            float currentRate = 0f;

            for (int i = 0; i < AttackPatternParams.Length; i++)
            {
                currentRate += AttackPatternParams[i].OccurrenceRate;
                if (randomPoint <= currentRate)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}
