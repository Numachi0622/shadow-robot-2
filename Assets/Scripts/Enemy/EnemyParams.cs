using UnityEngine;

[CreateAssetMenu(fileName = "EnemyParams", menuName = "ScriptableObjects/EnemyParams")]
public class EnemyParams : CharacterParams
{
    public float MoveSpeed;
    public float SearchRange;
    public float AttackRange;
}
