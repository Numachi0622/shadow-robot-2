using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Interface;
using Player;
using UniRx;
using UniRx.Triggers;
using UnityEditor.IMGUI.Controls;
using Utility;

public class PlayerAttacker : MonoBehaviour, IAttackable
{
    [SerializeField] private Collider _attackCollider;
    [SerializeField] private PunchGaugePresenter _punchGaugePresenter;

    private AttackPoint _attackPoint;
    private AttackInfo _attackInfo;
    public AttackInfo AttackInfo => _attackInfo;

    public void Initialize(CharacterParams characterParams)
    {
        _attackPoint = characterParams.AttackPoint;
        _attackInfo.AttackPoint = _attackPoint;
        _punchGaugePresenter.Initialize();

        _attackCollider.OnTriggerEnterAsObservable()
            .Where(target => target.GetComponent<IAttackable>() != null)
            .Select(_ => Mathf.RoundToInt(Mathf.Min(_attackInfo.AttackVelocity, GameConst.MAX_PUNCH_VELOCITY)))
            .Subscribe(value => _punchGaugePresenter.AddPunchPoint(value))
            .AddTo(this);
    }

    private void SetAttackInfo(float velocity, AttackType attackType)
    {
        _attackInfo.AttackVelocity = velocity;
        _attackInfo.AttackType = attackType;
    }
    
    public void Attack(Vector3 dir, float velocity)
    {
        SetAttackInfo(velocity, AttackType.PlayerToEnemyNormal);
        if (_attackCollider.enabled)
        {
            return;
        }
        _attackCollider.enabled = true;
        // debug
        _attackCollider.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = DebugConst.Instance.AttackingHandColor;
    }

    public void AttackEnd()
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

    public void Attack(float waitTime = 0) { }
}

public struct AttackInfo
{
    public AttackPoint AttackPoint;
    public Vector3 AttackDirection;
    public float AttackVelocity;
    public AttackType AttackType;
}

public enum AttackType
{
    PlayerToEnemyNormal,
    EnemyToPlayerNormal,
}