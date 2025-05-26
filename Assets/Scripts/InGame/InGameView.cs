using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class InGameView : MonoBehaviour
{
    [SerializeField] private GameObject _selectView;
    [SerializeField] private Material _transitionMat;
    
    [SerializeField] private GameObject _inGameView;
    [SerializeField] private TextMeshProUGUI _countDownText;

    private float _transitionRadius;
    private Sequence _transitionSequence;

    public void Initialize()
    {
        _selectView.SetActive(true);
        _inGameView.SetActive(false);
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
}
