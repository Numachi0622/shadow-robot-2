using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Interface;
using Utility;

public class Attacker : MonoBehaviour, IAttackable
{
    [SerializeField] private Collider _attackCollider;
    private AttackPoint _attackPoint;
    public AttackPoint AttackPoint => _attackPoint;

    public void Initialize(CharacterParams characterParams)
    {
        _attackPoint = characterParams.AttackPoint;
    }

    public void Attack()
    {
        if (_attackCollider.enabled) return;
        _attackCollider.enabled = true;
        // debug
        _attackCollider.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = DebugConst.Instance.AttackingHandColor;
    }

    public void EndAttack()
    {
        _attackCollider.enabled = false;
        // debug
        _attackCollider.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = DebugConst.Instance.DefaultHandColor;
    }
    
    public async UniTaskVoid AttackAsync(float waitTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        _attackCollider.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(GameConst.COLLIDER_ACTIVE_TIME));
        _attackCollider.enabled = false;
    }
}
