using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using Utility;

public class BodySourceManager : Singleton<BodySourceManager>
{
    [SerializeField] private TrackingDebugView _debugView;
    private KinectSensor _sensor;
    public KinectSensor Sensor => _sensor;
    private BodyFrameReader _reader;
    private Body[] _data = null;
    private ReactiveCollection<Body> _trackedData = new ReactiveCollection<Body>();
    public Body[] GetData() => _data;
    public IReadOnlyReactiveCollection<Body> TrackedData => _trackedData;
    
    public Windows.Kinect.Vector4 FloorClipPlane { get; private set; }

    public override void Initialize()
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
        
        Bind();
        
        base.Initialize();
    }

    private void Bind()
    {
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
                    ObserveBodies();
                
                    frame.Dispose();
                    frame = null;
                }
            })
            .AddTo(this);

        _trackedData.ObserveCountChanged()
            .Subscribe(_debugView.UpdateTrackedCountView)
            .AddTo(this);
    }

    private void ObserveBodies()
    {
        if (_data == null) return;
        
        for(var i = 0; i < _data.Length; i++)
        {
            var body = _data[i];
            if (body == null) continue;

            if (body.IsTracked)
            {
                if(_trackedData.Contains(body)) continue; 
                _trackedData.Add(body);
                continue;
            }
            
            if(!_trackedData.Contains(body)) continue;
            _trackedData.Remove(body);
        }
        
        _debugView.UpdateTrackingView(_data);
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
