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
    private static readonly Dictionary<(int, int, JointType, DataType), string> _jointAddress = new();
    private static readonly Dictionary<(int, int), string> _flagAddress = new();
    
    static OscAddress()
    {
        for (int deviceId = 0; deviceId < MaxDevicesCount; deviceId++)
        {
            for (int trackedId = 0; trackedId < MaxTrackedCount; trackedId++)
            {
                _flagAddress[(deviceId, trackedId)] = $"/{deviceId}/{trackedId}/isTracked";
                
                foreach (JointType joint in Enum.GetValues(typeof(JointType)))
                {
                    var rotationKey = (deviceId, personId: trackedId, joint, DataType.Rotation);
                    var jointName = ToCamelCase(joint.ToString());
                    _jointAddress[rotationKey] = $"/{deviceId}/{trackedId}/joint/rotation/{jointName}";
                    
                    var positionKey = (deviceId, personId: trackedId, joint, DataType.Position);
                    _jointAddress[positionKey] = $"/{deviceId}/{trackedId}/joint/position/{jointName}";
                }
            }
        }
    }
    
    public static string GetRotationAddress(int deviceId, int personId, JointType joint)
    {
        return _jointAddress[(deviceId, personId, joint, DataType.Rotation)];
    }
    
    public static string GetPositionAddress(int deviceId, int personId, JointType joint)
    {
        return _jointAddress[(deviceId, personId, joint, DataType.Position)];
    }
    
    public static string GetFlagAddress(int deviceId, int personId)
    {
        return _flagAddress[(deviceId, personId)];
    }
    
    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToLower(str[0]) + str.Substring(1);
    }
}