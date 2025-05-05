using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterParams : ScriptableObject
{
    public int MaxHp;
    public AttackPoint AttackPoint;
}

[Serializable]
public struct AttackPoint
{
    [SerializeField] private int _minAttackPoint;
    [SerializeField] private int _maxAttackPoint;
    public int RandomValue => Random.Range(_minAttackPoint, _maxAttackPoint + 1);
}
