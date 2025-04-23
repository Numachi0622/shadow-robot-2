using UnityEngine;
using System.Collections;
using Windows.Kinect;
using UniRx;
using UniRx.Triggers;

public class BodySourceManager : MonoBehaviour 
{
    private KinectSensor _sensor;
    public KinectSensor Sensor => _sensor;
    private BodyFrameReader _reader;
    private Body[] _data = null;
    
    public Body[] GetData()
    {
        return _data;
    }
    
    public Windows.Kinect.Vector4 FloorClipPlane { get; private set; }
    

    void Start () 
    {
        _sensor = KinectSensor.GetDefault();

        if (_sensor != null)
        {
            _reader = _sensor.BodyFrameSource.OpenReader();
            
            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }
        }
        
        this.UpdateAsObservable()
            .Where(_ => _reader != null)
            .Subscribe(_ =>
            {
                var frame = _reader.AcquireLatestFrame();
                if (frame != null)
                {
                    if (_data == null)
                    {
                        _data = new Body[_sensor.BodyFrameSource.BodyCount];
                    }
                
                    frame.GetAndRefreshBodyData(_data);
                    FloorClipPlane = frame.FloorClipPlane;
                
                    frame.Dispose();
                    frame = null;
                }
            })
            .AddTo(this);
    }
    
    void OnDestroy()
    {
        if (_reader != null)
        {
            _reader.Dispose();
            _reader = null;
        }
        
        if (_sensor != null)
        {
            if (_sensor.IsOpen)
            {
                _sensor.Close();
            }
            
            _sensor = null;
        }
    }
}
