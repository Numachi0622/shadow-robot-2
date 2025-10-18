using UnityEngine;

namespace SynMotion
{
    public interface IMotionSender
    {
        public void SendFlag(string address, int flag);
        public void SendMotion(string address, Quaternion rotation);

        public void SendPosition(string address, Vector3 position);
    }
}