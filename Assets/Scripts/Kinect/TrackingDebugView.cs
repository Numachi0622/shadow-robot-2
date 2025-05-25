using Windows.Kinect;
using UnityEngine;
using UnityEngine.UI;

public class TrackingDebugView : MonoBehaviour
{
    [SerializeField] private Text[] _trackingTexts = new Text[6];
    [SerializeField] private Color _trackedColor;
    [SerializeField] private Color _untrackedColor;
    
    public void UpdateTrackingView(Body[] bodies)
    {
        if(!gameObject.activeSelf) return;
        if(bodies == null) return;
        
        for (int i = 0; i < _trackingTexts.Length; i++)
        {
            var isTracked = bodies[i].IsTracked;
            _trackingTexts[i].text = isTracked.ToString();
            
            var color = isTracked ? _trackedColor : _untrackedColor;
            _trackingTexts[i].color = color;
        }
    }
}
