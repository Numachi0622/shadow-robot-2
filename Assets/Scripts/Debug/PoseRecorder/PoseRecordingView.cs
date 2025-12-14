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
        [SerializeField] private TMP_Text _poseDataPathText;
        [SerializeField] private TMP_Text _matchResultText;
        
        public Button RecordStartButton => _recordStartButton;

        public (string fileName, float recordTime) RecordInfo()
        {
            return (_fileNameInputField.text, float.Parse(_recordTimeInputField.text));
        }

        public void Initialize()
        {
            _dataTestButton.gameObject.SetActive(false);
            _matchResultText.gameObject.SetActive(false);
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

        public void SetPoseDataView(string path)
        {
            _poseDataPathText.text = path;
            _dataTestButton.gameObject.SetActive(true);
        }
        
        public void SetMatchResultView(string result)
        {
            _matchResultText.gameObject.SetActive(true);
            _matchResultText.text = result;
        }
    }
}