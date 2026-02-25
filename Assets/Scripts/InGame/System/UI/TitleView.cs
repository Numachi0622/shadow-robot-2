using System;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.System.UI
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] private InputField _fileInputField;
        [SerializeField] private Button _registerButton;
        [SerializeField] private Button[] _selectTextureButton;

        private Graphic _currentSelectButton;

        public IObservable<string> OnClickRegisterButton => _registerButton
            .OnClickAsObservable()
            .Select(_ => _fileInputField.text);

        public IObservable<int> OnClickSelectTextureButton => _selectTextureButton
                .Select((button, index) => button.OnClickAsObservable().Select(_ => index))
                .Merge();

        public void UpdateSelectState(int idx)
        {
            if (_currentSelectButton)
            {
                _currentSelectButton.color = Color.white;
            }
            _currentSelectButton = _selectTextureButton[idx].image;
            _currentSelectButton.color = Color.yellow;
        }

        public void UpdateTextureName(int playerId, PlayerTextureContext context)
        {
            var texture1Index = playerId * 2;
            var texture2Index = playerId * 2 + 1;

            if (texture1Index < _selectTextureButton.Length)
            {
                var text = _selectTextureButton[texture1Index].GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    text.text = context.Texture1 != null ? context.Texture1.name : "Empty";
                }
            }

            if (texture2Index < _selectTextureButton.Length)
            {
                var text = _selectTextureButton[texture2Index].GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    text.text = context.Texture2 != null ? context.Texture2.name : "Empty";
                }
            }
        }
    }
}