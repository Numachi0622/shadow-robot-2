using System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class TestEnemyAttackObserver : AttackObserver<Unit>
    {
        public override void Observe()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _onAttackStart.OnNext(Unit.Default);
            }
        }
    }
}