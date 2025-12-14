using UnityEngine;

namespace Utility.Extensions
{
    public static class QuaternionExtensions
    {
        public static Quaternion ToQuaternion(this Windows.Kinect.Vector4 vector, Quaternion comp)
        {
            return Quaternion.Inverse(comp) * new Quaternion(-vector.X, -vector.Y, vector.Z, vector.W);
        }

        public static Quaternion Add(this Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.x + b.x,
                a.y + b.y,
                a.z + b.z,
                a.w + b.w
            );
        } 
        
        public static Quaternion Divide(this Quaternion a, float d)
        {
            d = d == 0f ? 1f : d;
            return new Quaternion(
                a.x / d,
                a.y / d,
                a.z / d,
                a.w / d
            );
        } 
    }
}