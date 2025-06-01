using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class RocketPunchView : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _inGameView;
    [SerializeField] private GameObject _rocketPunchView;
    [SerializeField] private RectTransform _targetTransform;
    [SerializeField] private Image _chargeGauge;
    [SerializeField] private TextMeshProUGUI _chargeMsgText;
    [SerializeField] private TextMeshProUGUI _chargeValueText;
    private float _predictionDistance = 10f;
    private float _prevChargeRate;

    private string[] _chargeMsg = new[]
    {
        "足を交互に上げてチャージ！",
        "発射！！"
    };
    private Sequence _chargeGaugeSequence;
    private Sequence _chargeMsgSequence;
    private bool _isCharging = false;
    public bool IsCharging => _isCharging;

    public void Initialize()
    {
        _chargeGauge.fillAmount = 0f;
    }

    public void ShowView()
    {
        _rocketPunchView.SetActive(true);
        _inGameView.SetActive(false);

        _chargeMsgText.text = _chargeMsg[0];
        _chargeMsgText.transform.localScale = Vector3.one;
        _chargeMsgSequence?.Kill();
        _chargeMsgSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_chargeMsgText.transform.DOScale(Vector3.one * 1.1f, 0.4f))
            .SetLoops(-1, LoopType.Yoyo);
    }
    
    public void UpdateTarget(Vector3 dir)
    {
        if(!_targetTransform.gameObject.activeInHierarchy) return;
        var targetWorld = _camera.transform.position + dir * _predictionDistance;
        var screenPos = _camera.WorldToScreenPoint(targetWorld);
        _targetTransform.position = screenPos;
    }

    public void HideView()
    {
        _inGameView.SetActive(true);
        _rocketPunchView.SetActive(false);
        _chargeMsgSequence?.Kill();
    }

    public void UpdateChargeGauge(float value)
    {
        _chargeValueText.text = $"{Mathf.RoundToInt(value)}%";
        
        var rate = value / GameConst.MAX_ROCKET_PUNCH_CHARGE;
        if (rate < _prevChargeRate)
        {
            _chargeGauge.fillAmount = rate;
            _prevChargeRate = rate;
            return;
        }

        _prevChargeRate = rate;
        _chargeGaugeSequence?.Kill();
        _chargeGaugeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_chargeGauge.DOFillAmount(rate, 0.3f)
                .OnStart(() => _isCharging = true)
                .OnComplete(() => _isCharging = false));
    }

    public void ShowLaunchMsg()
    {
        _chargeMsgText.text = _chargeMsg[1];
    }
}
