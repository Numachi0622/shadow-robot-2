using System;
using UnityEngine;

public class JumpCalibrationModel
{
    private float[] _positionData;
    private float _calibratePos = 0f;
    public float CalibratePosition => _calibratePos;
    private int _currentIndex = 0;
    private bool _isCalibrating = false;
    public bool IsCalibrating => _isCalibrating;
    
    private const int Data_Num = 100;

    public Action OnCalibrated;

    public JumpCalibrationModel()
    {
        _positionData = new float[Data_Num];
    }

    public void SetCalibration()
    {
        if(_isCalibrating) return;
        _isCalibrating = true;
    }

    public void Calibrate(float data)
    {
        if(!_isCalibrating) return;

        _positionData[_currentIndex] = data;
        _currentIndex++;

        if (_currentIndex >= Data_Num)
        {
            _isCalibrating = false;
            
            var sum = 0f;
            for(var i = 0; i < Data_Num; i++)
            {
                sum += _positionData[i];
            }
            _calibratePos = sum / Data_Num;
            
            OnCalibrated?.Invoke();
        }
    }
}
