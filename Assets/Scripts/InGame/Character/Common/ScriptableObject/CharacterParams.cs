using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "CharacterParams", menuName = "ScriptableObjects/Character/CharacterParams", order = 1)]
    public class CharacterParams : ScriptableObject
    {
        public int MaxHp;
        public AttackPoint AttackPoint;
        public Color DamagedColor;
    }

    [Serializable]
    public struct AttackPoint
    {
        [SerializeField] private int _minAttackPoint;
        [SerializeField] private int _maxAttackPoint;
        public int MinAttackPoint => _minAttackPoint;
        public int MaxAttackPoint => _maxAttackPoint;
        public int RandomValue => Random.Range(_minAttackPoint, _maxAttackPoint + 1);
    }
}