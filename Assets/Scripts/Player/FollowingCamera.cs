using UniRx;
using UniRx.Triggers;
using UnityEngine;
using DG.Tweening;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxSpeedDistance;
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _selectViewCamera;
    [SerializeField] private Camera _inGameViewCamera;
    private Vector3 _offset;
    public void Initialize()
    {
        //_offset = transform.position - _target.position;
        _selectViewCamera.gameObject.SetActive(true);
        _inGameViewCamera.gameObject.SetActive(false);

        this.UpdateAsObservable()
            .Subscribe(_ => Follow())
            .AddTo(this);
    }

    private void Follow()
    {
        if(_target == null) return;
        
        var currentPos = transform.position;
        var targetPos = _target.position + _offset;
        var dist = Vector3.Distance(currentPos, targetPos);
        var followSpeed = _maxSpeed * Mathf.InverseLerp(0f, _maxSpeedDistance, dist);
        transform.position = Vector3.MoveTowards(currentPos, targetPos, followSpeed * Time.deltaTime);
    }

    public void SetInGameCamera()
    {
        _selectViewCamera.gameObject.SetActive(false);
        _inGameViewCamera.gameObject.SetActive(true);
    }
}
