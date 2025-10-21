using UnityEngine;
using SynMotion;

namespace InGame.Character
{
    public class PlayerMover : Mover
    {
        public PlayerMover(Transform targetTransform) : base(targetTransform)
        {
        }

        public override void Move(Vector3 pos)
        {
            var movedPos = pos;
            _targetTransform.position = movedPos;
        }
    }   
}
