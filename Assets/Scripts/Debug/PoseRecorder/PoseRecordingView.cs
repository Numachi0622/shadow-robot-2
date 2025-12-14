using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShadowRobotDebug
{
    public class PoseRecordingView : MonoBehaviour
    {
        [SerializeField] private Button _recordStartButton;
        [SerializeField] private Button _dataTestButton;
        [SerializeField] private TMP_InputField _fileNameInputField;
        [SerializeField] private TMP_InputField _recordTimeInputField;
        
        public Button RecordStartButton => _recordStartButton;

        public (string fileName, float recordTime) RecordInfo()
        {
            return (_fileNameInputField.text, float.Parse(_recordTimeInputField.text));
        }

        public void Initialize()
        {
            
        }

        public void SetButtonView(string text, Color color)
        {
            _recordStartButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            _recordStartButton.GetComponent<Image>().color = color;
        }

        public void ResetButtonView()
        {
            SetButtonView("Record Start", Color.white);
        }
    }
}