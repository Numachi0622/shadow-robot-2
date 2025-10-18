using DEMAFilter;
using UnityEngine;

namespace Utility
{
    public static class VectorExtensions
    {
        private static readonly Vector3DEMAFilter _filter = new Vector3DEMAFilter(GameConst.VECTOR3_DEMA_ALPHA);
        public static Quaternion ToQuaternion(this Windows.Kinect.Vector4 vector, Quaternion comp)
        {
            return Quaternion.Inverse(comp) * new Quaternion(-vector.X, -vector.Y, vector.Z, vector.W);
        }

        public static Windows.Kinect.Vector4 ToMirror(this Windows.Kinect.Vector4 vector)
        {
            return new Windows.Kinect.Vector4()
            {
                X = vector.X,
                Y = -vector.Y,
                Z = -vector.Z,
                W = vector.W
            };
        }

        public static Vector3 DEMAFilter(this Vector3 input)
        {
            return _filter.Filter(input);
        }
    }
}