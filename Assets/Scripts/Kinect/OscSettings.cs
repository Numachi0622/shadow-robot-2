using UnityEngine;

[CreateAssetMenu(fileName = "OscSettings", menuName = "ScriptableObjects/OscSettings")]
public class OscSettings : ScriptableObject
{
    [SerializeField] private string _ipAddress = "127.0.0.1";
    [SerializeField] private int _port = 9000;
    public string IpAddress => _ipAddress;
    public int Port => _port;
}
