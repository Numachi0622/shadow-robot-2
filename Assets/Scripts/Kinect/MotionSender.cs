using OscCore;
using UnityEngine;
using SynMotion;

public class MotionSender : IMotionSender
{
    private OscClient _sender;
    private OscWriter _writer = new OscWriter();
    
    public MotionSender(string ipAddress, int port)
    {
        _sender = new OscClient(ipAddress, port);
    }
    
    public void SendMotion(string address, Quaternion rot)
    {
        _sender.Send(address, rot);
    }
}
