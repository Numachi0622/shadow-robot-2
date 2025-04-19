using System;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;

public class KinectTest : MonoBehaviour
{
    private Device _kinect;
    
    private void Start()
    {
        _kinect = Device.Open(0);
        _kinect.StartCameras(new DeviceConfiguration()
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });
    }

    private void OnDestroy() => _kinect.StopCameras();
}
