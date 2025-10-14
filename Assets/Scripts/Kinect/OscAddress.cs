using System;
using System.Collections.Generic;
using Windows.Kinect;

public static class OscAddress
{
    private enum DataType
    {
        Rotation,
        Position,
    }
    
    private const int MaxDevicesCount = 2;
    private const int MaxTrackedCount = 2;
    private static readonly Dictionary<(int, int, JointType, DataType), string> _address = new();
    
    static OscAddress()
    {
        for (int deviceId = 0; deviceId < MaxDevicesCount; deviceId++)
        {
            for (int trackedId = 0; trackedId < MaxTrackedCount; trackedId++)
            {
                foreach (JointType joint in Enum.GetValues(typeof(JointType)))
                {
                    var rotationKey = (deviceId, personId: trackedId, joint, DataType.Rotation);
                    var jointName = ToCamelCase(joint.ToString());
                    _address[rotationKey] = $"/{deviceId}/{trackedId}/joint/rotation/{jointName}";
                    
                    var positionKey = (deviceId, personId: trackedId, joint, DataType.Position);
                    _address[positionKey] = $"/{deviceId}/{trackedId}/joint/position/{jointName}";
                }
            }
        }
    }
    
    public static string GetRotationAddress(int deviceId, int personId, JointType joint)
    {
        return _address[(deviceId, personId, joint, DataType.Rotation)];
    }
    
    public static string GetPositionAddress(int deviceId, int personId, JointType joint)
    {
        return _address[(deviceId, personId, joint, DataType.Position)];
    }
    
    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToLower(str[0]) + str.Substring(1);
    }
}