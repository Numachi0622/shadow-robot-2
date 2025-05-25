using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RocketPunchDetector : MonoBehaviour
{
    private int _detectCount = 0;
    public void Start()
    {
        var layerMask = LayerMask.GetMask("Detector");
        var colliders = new Collider[2];
        
        this.UpdateAsObservable()
            .Select(count => _detectCount = Physics.OverlapSphereNonAlloc(
                transform.position, 0.5f, colliders, layerMask))
            .Subscribe(count =>
            {
                _detectCount = count;
            })
            .AddTo(this);
    }
}