using UnityEngine;

namespace InGame.Character
{
    public interface IMovable
    {
        public void Move(Vector3 direction);
        public void Rotate(Vector3 direction);
    }
}