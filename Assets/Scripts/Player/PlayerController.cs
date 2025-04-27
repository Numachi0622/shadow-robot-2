using System;
using Cysharp.Threading.Tasks;
using Player;
using UniRx;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerParams _params;
    [SerializeField] private Attacker _leftAttacker, _rightAttacker;
    [SerializeField] private Transform _leftHand, _rightHand;
    private void Start()
    {
        // Initialize
        var leftHandAttackDetector = new HandAttackDetector(_leftHand, _params.AttackableVelocity);
        var rightHandAttackDetector = new HandAttackDetector(_rightHand, _params.AttackableVelocity);
        
        // Bind
        leftHandAttackDetector.OnAttack
            .Subscribe(_ => _leftAttacker.Attack(),
                () => _leftAttacker.EndAttack())
            .AddTo(this);

        rightHandAttackDetector.OnAttack
            .Subscribe(_ => _rightAttacker.Attack(),
                () => _rightAttacker.EndAttack())
            .AddTo(this);
    }
}
