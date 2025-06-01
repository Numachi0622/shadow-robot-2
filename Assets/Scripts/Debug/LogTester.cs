using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public class LogTester : MonoBehaviour
{
    [SerializeField] private Transform _handLeft, _handRight;
    [SerializeField] private Transform _lowerArmLeft, _lowerArmRight;
    [SerializeField] private Transform _body;
    [SerializeField] private float _threshold = 0.8f;

    private void Update()
    {
        var leftHandDir = ((-_handLeft.right - _lowerArmLeft.right) * 0.5f).normalized;
        var rightHandDir = ((-_handRight.right - _lowerArmRight.right) * 0.5f).normalized;
        
        bool isReadyRocketPunch = Vector3.Dot(leftHandDir, _body.forward) > _threshold;
        var color = isReadyRocketPunch ? Color.green : Color.red;
        Debug.DrawLine(_handLeft.position, _handLeft.position + leftHandDir * 10f, color);
        
        isReadyRocketPunch = Vector3.Dot(rightHandDir, _body.forward) > _threshold;
        color = isReadyRocketPunch ? Color.green : Color.red;
        Debug.DrawLine(_handRight.position, _handRight.position + leftHandDir * 10f, color);
        
    }
}
