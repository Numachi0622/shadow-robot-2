using UniRx;
using UniRx.Triggers;
using UnityEngine;
using DG.Tweening;
using UnityEditor.ShaderGraph.Internal;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxSpeedDistance;
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _selectViewCamera;
    [SerializeField] private Camera _inGameViewCamera;
    private Vector3 _offset;
    private Vector3 _initialPos;
    private Vector3 _initialRot;
    private Vector3 _rocketPunchReadyPos = new Vector3(0, 1.75f, 0);
    private Sequence _rocketPunchrReadySequence;
    
    public void Initialize()
    {
        //_offset = transform.position - _target.position;
        _initialPos = transform.localPosition;
        _initialRot = transform.localEulerAngles;
        _selectViewCamera.gameObject.SetActive(true);
        _inGameViewCamera.gameObject.SetActive(false);

        // this.UpdateAsObservable()
        //     .Subscribe(_ => Follow())
        //     .AddTo(this);
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

    public void SetRocketPunchCamera()
    {
        _rocketPunchrReadySequence? .Kill();
        _rocketPunchrReadySequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(transform.DOLocalMove(_rocketPunchReadyPos, 0.3f))
            .Join(transform.DOLocalRotate(_initialRot, 0.3f));
    }
    
    public void CancelRocketPunchCamera()
    {
        _rocketPunchrReadySequence?.Kill();
        _rocketPunchrReadySequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(transform.DOLocalMove(_initialPos, 0.3f))
            .Join(transform.DOLocalRotate(_initialRot, 0.3f));
    }
}
