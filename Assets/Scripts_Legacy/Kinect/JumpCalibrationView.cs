using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class JumpCalibrationView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _calibrationText;

    public void ShowCalibrationText()
    {
        if(_calibrationText.gameObject.activeSelf) return;
        _calibrationText.gameObject.SetActive(true);
    }
    
    public async UniTask HideCalibrationText()
    {
        await UniTask.DelayFrame(1);
        _calibrationText.gameObject.SetActive(false);
    }
}
