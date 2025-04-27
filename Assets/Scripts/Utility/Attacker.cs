using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Interface;

public class Attacker : MonoBehaviour, IAttackable
{
    [SerializeField] private Collider _attackCollider;

    public void Attack()
    {
        if (_attackCollider.enabled) return;
        _attackCollider.enabled = true;
        // debug
        _attackCollider.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void EndAttack()
    {
        _attackCollider.enabled = false;
        // debug
        _attackCollider.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;
    }
    
    public async UniTaskVoid AttackAsync(float waitTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        _attackCollider.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(GameConst.COLLIDER_ACTIVE_TIME));
        _attackCollider.enabled = false;
    }
}
