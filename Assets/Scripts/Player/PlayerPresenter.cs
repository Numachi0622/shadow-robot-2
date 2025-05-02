using System;
using Cysharp.Threading.Tasks;
using Player;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] private PlayerParams _params;
    [SerializeField] private Attacker _leftAttacker, _rightAttacker;
    [SerializeField] private Transform _leftHand, _rightHand;
    [SerializeField] private PlayerKinectMotion _kinectMotion;
    public void Initialize()
    {
        // Initialize
        var leftHandAttackDetector = new HandAttackDetector(_leftHand, _params.AttackableVelocity);
        var rightHandAttackDetector = new HandAttackDetector(_rightHand, _params.AttackableVelocity);
        _kinectMotion.Initialize();
        
        // Bind
        leftHandAttackDetector.OnAttack
            .Subscribe(_ => _leftAttacker.Attack())
            .AddTo(this);
        
        leftHandAttackDetector.OnAttackEnd
            .Subscribe(_ => _leftAttacker.EndAttack())
            .AddTo(this);

        rightHandAttackDetector.OnAttack
            .Subscribe(_ => _rightAttacker.Attack())
            .AddTo(this);
        
        rightHandAttackDetector.OnAttackEnd
            .Subscribe(_ => _rightAttacker.EndAttack())
            .AddTo(this);
    }
}
