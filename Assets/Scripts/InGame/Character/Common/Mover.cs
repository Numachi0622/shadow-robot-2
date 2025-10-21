using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace InGame.Character
{
    public abstract class Mover : IMovable
    {
        protected Transform _targetTransform;

        public Mover(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public virtual void Move(Vector3 direction)
        {
        }
    }
}