using System;
using InGame.Character;
using SynMotion;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    [SerializeField] private MovementTransforms _movementTransforms;

    private PlayerMovement _movement;
    private MotionReceiver _receiver;

    private void Start()
    {
        var synMotion = new SynMotionSystem();
        _receiver = new MotionReceiver(9000, synMotion);
        _movement = new PlayerMovement(_movementTransforms, synMotion);
    }

    private void Update()
    {
        _receiver.UpdateMotion();
        
        _movement.UpdateMotion();
    }
}
