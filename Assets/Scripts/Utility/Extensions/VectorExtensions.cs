using Windows.Kinect;
using UnityEngine;

namespace Utility.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3(this CameraSpacePoint vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
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
    }
}