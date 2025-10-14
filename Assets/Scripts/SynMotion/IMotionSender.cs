using UnityEngine;

namespace SynMotion
{
    public interface IMotionSender
    {
        public void SendMotion(string address, Quaternion rotation);

        public void SendPosition(string address, Vector3 position);
    }
}