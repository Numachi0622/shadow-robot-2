using Windows.Kinect;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.Character
{
    public class TrackingDebugView : MonoBehaviour
    {
        [SerializeField] private Text _trackedCountText;
        [SerializeField] private Text _fpsText;
        [SerializeField] private Text[] _trackingTexts = new Text[6];
        [SerializeField] private Color _trackedColor;
        [SerializeField] private Color _untrackedColor;

        public void UpdateTrackingView(Body[] bodies)
        {
            if (!gameObject.activeSelf) return;
            if (bodies == null) return;

            for (int i = 0; i < _trackingTexts.Length; i++)
            {
                var isTracked = bodies[i].IsTracked;
                _trackingTexts[i].text = isTracked.ToString();

                var color = isTracked ? _trackedColor : _untrackedColor;
                _trackingTexts[i].color = color;
            }
        }

        public void UpdateTrackedCountView(int count)
        {
            _trackedCountText.text = $"Tracking Count: {count}";
        }

        public void UpdateFpsView(int fps)
        {
            if (!gameObject.activeSelf) return;
            _fpsText.text = $"FPS: {fps}";
        }
    }
}
