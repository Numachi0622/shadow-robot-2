using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class DebugParamsPresenter : MonoBehaviour
{
    [SerializeField] private DebugParamsView _view;
    private DebugParamsModel _model;
    public DebugParamsModel Model => _model;

    public void Initialize()
    {
        _model = new DebugParamsModel();

        // view -> model
        _view.RightIdChangeButton.OnClickAsObservable()
            .Subscribe(_ => _model.SetRightTrackedId())
            .AddTo(this);

        _view.LeftIdChangeButton.OnClickAsObservable()
            .Subscribe(_ => _model.SetLeftTrackedId())
            .AddTo(this);
        
        // model -> view
        _model.TrackedCount
            .Subscribe(count => _view.UpdateTrackedCount(count))
            .AddTo(this);
        
        _model.RightTrackedId
            .Subscribe(id => _view.UpdateRightTrackedId(id))
            .AddTo(this);

        _model.LeftTrackedId
            .Subscribe(id => _view.UpdateLeftTrackedId(id))
            .AddTo(this);
    }

    public void SetCount(int trackedCount)
    {
        _model.SetTrackedCount(trackedCount);
    }
}
