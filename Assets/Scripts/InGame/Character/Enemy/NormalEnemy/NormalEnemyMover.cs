using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyMover : Mover
    {
        public NormalEnemyMover(Transform targetTransform) : base(targetTransform)
        {
        }

        public override void Move(Vector3 pos)
        {
            var movedPos = pos;
            _targetTransform.position = movedPos;
        }

        public override void Rotate(Vector3 direction)
        {
            var rotation = Quaternion.LookRotation(direction);
            _targetTransform.rotation = rotation;
        }
    }
}