using UnityEngine;
using UnityEngine.UI;

public class DebugParamsView : MonoBehaviour
{
    [SerializeField] private Text _trackedCountText;
    [SerializeField] private Text _rightTrackedId;
    [SerializeField] private Text _leftTrackedID;
    [SerializeField] private Button _rightIdChangeButton;
    public Button RightIdChangeButton => _rightIdChangeButton;
    [SerializeField] private Button _leftIdChangeButton;
    public Button LeftIdChangeButton => _leftIdChangeButton;
    
    public void UpdateTrackedCount(int count)
    {
        _trackedCountText.text = $"Tracked Count: {count}";
    }
    
    public void UpdateRightTrackedId(int id)
    {
        _rightTrackedId.text = $"Tracked ID: {id}";
    }
    
    public void UpdateLeftTrackedId(int id)
    {
        _leftTrackedID.text = $"Tracked ID: {id}";
    }
}
