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

    public void SendFlag(string address, int flag)
    {
        _sender.Send(address, flag);
    }

    public void SendMotion(string address, Quaternion rot)
    {
        _sender.Send(address, rot);
    }

    public void SendPosition(string address, Vector3 position)
    {
        _sender.Send(address, position);
    }
}
