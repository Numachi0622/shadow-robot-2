public static class OscAddress
{
    private const int MaxDeviceCount = 2;
    private const int MaxTrackedCount = 2;
    
    // 2次元配列 [デバイスID][人のID]
    public static readonly string[][] SpineBaseRotation;
    public static readonly string[][] SpineMidRotation;
    public static readonly string[][] NeckRotation;
    public static readonly string[][] HeadRotation;
    public static readonly string[][] ShoulderLeftRotation;
    public static readonly string[][] ElbowLeftRotation;
    public static readonly string[][] WristLeftRotation;
    public static readonly string[][] HandLeftRotation;
    public static readonly string[][] ShoulderRightRotation;
    public static readonly string[][] ElbowRightRotation;
    public static readonly string[][] WristRightRotation;
    public static readonly string[][] HandRightRotation;
    public static readonly string[][] HipLeftRotation;
    public static readonly string[][] KneeLeftRotation;
    public static readonly string[][] AnkleLeftRotation;
    public static readonly string[][] FootLeftRotation;
    public static readonly string[][] HipRightRotation;
    public static readonly string[][] KneeRightRotation;
    public static readonly string[][] AnkleRightRotation;
    public static readonly string[][] FootRightRotation;
    public static readonly string[][] SpineShoulderRotation;
    public static readonly string[][] HandTipLeftRotation;
    public static readonly string[][] ThumbLeftRotation;
    public static readonly string[][] HandTipRightRotation;
    public static readonly string[][] ThumbRightRotation;
    public static readonly string[][] SpineMidPosition;
    
    static OscAddress()
    {
        SpineBaseRotation = new string[MaxDeviceCount][];
        SpineMidRotation = new string[MaxDeviceCount][];
        NeckRotation = new string[MaxDeviceCount][];
        HeadRotation = new string[MaxDeviceCount][];
        ShoulderLeftRotation = new string[MaxDeviceCount][];
        ElbowLeftRotation = new string[MaxDeviceCount][];
        WristLeftRotation = new string[MaxDeviceCount][];
        HandLeftRotation = new string[MaxDeviceCount][];
        ShoulderRightRotation = new string[MaxDeviceCount][];
        ElbowRightRotation = new string[MaxDeviceCount][];
        WristRightRotation = new string[MaxDeviceCount][];
        HandRightRotation = new string[MaxDeviceCount][];
        HipLeftRotation = new string[MaxDeviceCount][];
        KneeLeftRotation = new string[MaxDeviceCount][];
        AnkleLeftRotation = new string[MaxDeviceCount][];
        FootLeftRotation = new string[MaxDeviceCount][];
        HipRightRotation = new string[MaxDeviceCount][];
        KneeRightRotation = new string[MaxDeviceCount][];
        AnkleRightRotation = new string[MaxDeviceCount][];
        FootRightRotation = new string[MaxDeviceCount][];
        SpineShoulderRotation = new string[MaxDeviceCount][];
        HandTipLeftRotation = new string[MaxDeviceCount][];
        ThumbLeftRotation = new string[MaxDeviceCount][];
        HandTipRightRotation = new string[MaxDeviceCount][];
        ThumbRightRotation = new string[MaxDeviceCount][];
        SpineMidPosition = new string[MaxDeviceCount][];
        
        for (int deviceId = 0; deviceId < MaxDeviceCount; deviceId++)
        {
            SpineBaseRotation[deviceId] = new string[MaxTrackedCount];
            SpineMidRotation[deviceId] = new string[MaxTrackedCount];
            NeckRotation[deviceId] = new string[MaxTrackedCount];
            HeadRotation[deviceId] = new string[MaxTrackedCount];
            ShoulderLeftRotation[deviceId] = new string[MaxTrackedCount];
            ElbowLeftRotation[deviceId] = new string[MaxTrackedCount];
            WristLeftRotation[deviceId] = new string[MaxTrackedCount];
            HandLeftRotation[deviceId] = new string[MaxTrackedCount];
            ShoulderRightRotation[deviceId] = new string[MaxTrackedCount];
            ElbowRightRotation[deviceId] = new string[MaxTrackedCount];
            WristRightRotation[deviceId] = new string[MaxTrackedCount];
            HandRightRotation[deviceId] = new string[MaxTrackedCount];
            HipLeftRotation[deviceId] = new string[MaxTrackedCount];
            KneeLeftRotation[deviceId] = new string[MaxTrackedCount];
            AnkleLeftRotation[deviceId] = new string[MaxTrackedCount];
            FootLeftRotation[deviceId] = new string[MaxTrackedCount];
            HipRightRotation[deviceId] = new string[MaxTrackedCount];
            KneeRightRotation[deviceId] = new string[MaxTrackedCount];
            AnkleRightRotation[deviceId] = new string[MaxTrackedCount];
            FootRightRotation[deviceId] = new string[MaxTrackedCount];
            SpineShoulderRotation[deviceId] = new string[MaxTrackedCount];
            HandTipLeftRotation[deviceId] = new string[MaxTrackedCount];
            ThumbLeftRotation[deviceId] = new string[MaxTrackedCount];
            HandTipRightRotation[deviceId] = new string[MaxTrackedCount];
            ThumbRightRotation[deviceId] = new string[MaxTrackedCount];
            SpineMidPosition[deviceId] = new string[MaxTrackedCount];
            
            for (int personId = 0; personId < MaxTrackedCount; personId++)
            {
                // アドレス形式: /deviceId/personId/joint/rotation/jointName
                SpineBaseRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/spineBase";
                SpineMidRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/spineMid";
                NeckRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/neck";
                HeadRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/head";
                ShoulderLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/shoulderLeft";
                ElbowLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/elbowLeft";
                WristLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/wristLeft";
                HandLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/handLeft";
                ShoulderRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/shoulderRight";
                ElbowRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/elbowRight";
                WristRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/wristRight";
                HandRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/handRight";
                HipLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/hipLeft";
                KneeLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/kneeLeft";
                AnkleLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/ankleLeft";
                FootLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/footLeft";
                HipRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/hipRight";
                KneeRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/kneeRight";
                AnkleRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/ankleRight";
                FootRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/footRight";
                SpineShoulderRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/spineShoulder";
                HandTipLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/handTipLeft";
                ThumbLeftRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/thumbLeft";
                HandTipRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/handTipRight";
                ThumbRightRotation[deviceId][personId] = $"/{deviceId}/{personId}/joint/rotation/thumbRight";
                SpineMidPosition[deviceId][personId] = $"/{deviceId}/{personId}/joint/position/spineMid";
            }
        }
    }
}