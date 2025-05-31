using System;
using kinect = Windows.Kinect;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using TMPro;
using UniRx;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class InGameView : Singleton<InGameView>
{
    [SerializeField] private Material _transitionMat;

    // Select View
    [SerializeField] private GameObject _selectView;
    [SerializeField] private Color _disConnectColor;
    [SerializeField] private Image[] _trackingState;
    
    // in game view
    [SerializeField] private GameObject _inGameView;
    [SerializeField] private TextMeshProUGUI _countDownText;
    [SerializeField] private TextMeshProUGUI _warningText;
    [SerializeField] private RectTransform _bossGenerateViewTransform;
    [SerializeField] private TextMeshProUGUI _bossGenerateText;
    
    // result view
    [SerializeField] private CanvasGroup _resultBackground;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _gameClearText;

    private float _transitionRadius;
    private string[] _warningMessage = new []
    {
        "ジャンプして回避！", "移動して回避！"
    };
    private Sequence _transitionSequence;
    private Sequence _warningTextSequence;
    private Sequence _resultViewSequence;
    private Sequence _bossGenerateSequence;

    public override void Initialize()
    {
        _selectView.SetActive(true);
        ResetTrackingState();
        _inGameView.SetActive(false);
        
        base.Initialize();
    }

    private void ResetTrackingState()
    {
        foreach (var state in _trackingState)
        {
            state.color = _disConnectColor;
            state.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void UpdateTrackingStateView(IReadOnlyReactiveCollection<kinect.Body> bodies)
    {
        if(bodies == null) return;
        if (bodies.Count == 0)
        {
            ResetTrackingState();
            return;
        }

        foreach (var state in _trackingState)
        {
            state.color = Color.white;
            state.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    
    public void SetInGameView()
    {
        _selectView.SetActive(false);
        _inGameView.SetActive(true);
    }

    public void PlayTransition(Action onTransition = null, Action onComplete = null)
    {
        _transitionSequence?.Kill();
        
        _transitionSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.To(() => 1f, SetTransitionValue, 0f, 0.7f))
            .AppendCallback(() => onTransition?.Invoke())
            .AppendInterval(0.3f)
            .Append(DOTween.To(() => 0f, SetTransitionValue, 1f, 0.7f))
            .AppendCallback(() => onComplete?.Invoke());
    }

    private void SetTransitionValue(float value)
    {
        _transitionMat.SetFloat("_Radius", value);
    }

    public async UniTaskVoid UpdateCountDown(float delay, Action callback = null)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        var count = 3;
        _countDownText.gameObject.SetActive(true);
        _countDownText.text = count.ToString();
        
        await UniTask.Delay(TimeSpan.FromSeconds(1f)); 
        
        while (count > 1)
        {
            count--;
            _countDownText.text = count.ToString();
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }

        _countDownText.text = "Start!!";
        
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        _countDownText.gameObject.SetActive(false);
        
        callback?.Invoke();
    }
    
    public void ShowWarningText(int msgIndex)
    {
        var idx = Mathf.Clamp(msgIndex - 1, 0, _warningMessage.Length - 1);
        var defaultScale = Vector3.one;
        
        _warningTextSequence?.Kill();
        _warningTextSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_warningText.transform.DOScale(defaultScale * 1.2f, 0.5f)
                .SetLoops(3, LoopType.Yoyo)
                .OnStart(() =>
                {
                    _warningText.text = _warningMessage[idx];
                    _warningText.gameObject.SetActive(true);
                })
                .OnComplete(() =>
                {
                    _warningText.transform.localScale = defaultScale;
                    _warningText.gameObject.SetActive(false);
                }));
    }

    public void CancelWarningView()
    {
        _warningTextSequence?.Kill();
        _warningText.transform.localScale = Vector3.one;
        _warningText.gameObject.SetActive(false);
    }

    public void ShowBossGenerateView(Action onComplete = null)
    {
        _bossGenerateSequence?.Kill();

        _bossGenerateViewTransform.gameObject.SetActive(true);
        _bossGenerateSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_bossGenerateViewTransform.DOScaleX(1f, 0.5f))
            .Join(_bossGenerateText.DOFade(1f, 0.75f)
                .SetLoops(4, LoopType.Yoyo))
            .Append(_bossGenerateViewTransform.DOScaleX(0f, 0.5f))
            .AppendCallback(() =>
            {
                _bossGenerateViewTransform.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }

    public void ShowResultView(bool isClear, float delay = 0f)
    {
        _resultViewSequence?.Kill();
        _resultViewSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .AppendInterval(delay);
        
        if (!isClear)
        {
            _gameOverText.transform.localPosition = Vector2.up * 700f;
            _resultViewSequence
                .Append(_resultBackground.DOFade(1f, 1f))
                .Join(_gameOverText.transform.DOLocalMoveY(0f, 1f)
                    .SetEase(Ease.OutBack)
                    .OnStart(() =>
                    {
                        _gameOverText.gameObject.SetActive(true);
                    }));
            return;
        }

        _resultViewSequence
            .Append(_gameClearText.transform.DOScale(Vector3.one * 1.2f, 1f)
                .SetEase(Ease.OutBack)
                .OnStart(() =>
                {
                    _gameClearText.gameObject.SetActive(true);
                    _gameClearText.transform.localScale = Vector3.zero;
                }));
    }
}
