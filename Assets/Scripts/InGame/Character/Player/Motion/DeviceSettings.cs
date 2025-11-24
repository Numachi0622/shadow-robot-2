using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "DeviceSettings", menuName = "DeviceSettings")]
    public class DeviceSettings : ScriptableObject
    {
        public string IpAddress = "192.0.0.1";
        public int Port = 8080;
        public int MaxDeviceCount = 2;
        public int MaxTrackingCountPerDevice = 2;
    }
}