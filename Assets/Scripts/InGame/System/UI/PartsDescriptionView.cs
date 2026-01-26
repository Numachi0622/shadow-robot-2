using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace InGame.System.UI
{
    public class PartsDescriptionView : MonoBehaviour, IVisibilityController
    {
        [SerializeField] private CanvasGroup _rootCanvasGroup;
        [SerializeField] private CanvasGroup[] _backgroundCanvasGroups;
        [SerializeField] private CanvasGroup[] _descriptionPanelCanvasGroups;
        
        // todo: この辺りのリスト管理綺麗にする
        [SerializeField] private Image _leftHandImage;
        [SerializeField] private Image _rightHandImage;
        [SerializeField] private Image _footImage;
        private readonly List<List<Image>> _partsImageLists = new()
        {
            new (),
            new (),
            new ()
        };
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _defaultColor;

        private Sequence _partsActiveSequence;

        public bool IsActive => gameObject.activeSelf;
        
        public void Show()
        {
            if (gameObject.activeSelf) return;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (!gameObject.activeSelf) return;
        }

        public async UniTask ShowAsync(IVisibilityContext context = null)
        {
            Show();
            await _rootCanvasGroup.DOFade(1f, 0.5f).ToUniTask();
        }

        public async UniTask HideAsync(IVisibilityContext context = null)
        {
            Hide();
            await _rootCanvasGroup.DOFade(0f, 0.5f).ToUniTask();
        }

        public async UniTask ShowAndHideAsync(IReadOnlyList<(string title, string message)> descriptions)
        {
            var playerCount = descriptions.Count;
            if (playerCount <= 1 || playerCount > GameConst.MaxPlayerCount) return;
            
            // todo: 一旦クソコード、ここら辺あとで変えたい
            if (playerCount == 2)
            {
                _partsImageLists[0].Add(_leftHandImage);
                _partsImageLists[0].Add(_rightHandImage);
                _partsImageLists[1].Add(_footImage);
            }
            else
            {
                _partsImageLists[0].Add(_leftHandImage);
                _partsImageLists[1].Add(_rightHandImage);
                _partsImageLists[2].Add(_footImage);
            }
            
            await ShowAsync();
            
            // プレイヤー数に応じてUIを配置
            var widthPerPlayer = (float)GameConst.DisplayWidth / playerCount;
            for (var i = 0; i < playerCount; i++)
            {
                var bgRectTransform = _backgroundCanvasGroups[i].GetComponent<RectTransform>();
                var descriptionRectTransform = _descriptionPanelCanvasGroups[i].GetComponent<RectTransform>();
                
                var horizontalPos = (-GameConst.DisplayWidth / 2f) + widthPerPlayer * (i + 0.5f);
                bgRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthPerPlayer);
                bgRectTransform.localPosition = new Vector2(horizontalPos, 0f);
                descriptionRectTransform.localPosition = new Vector2(horizontalPos, descriptionRectTransform.localPosition.y);

                var title = _descriptionPanelCanvasGroups[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                title.text = "きみは" + descriptions[i].title + "だ！";
                
                var message = _descriptionPanelCanvasGroups[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                message.text = descriptions[i].message;
            }
            
            var bgList = new List<CanvasGroup>();
            var fadeInTasks = new Queue<UniTask>();
            var fadeOutTasks = new Queue<UniTask>();
            for (var i = 0; i < playerCount; i++)
            {
                // 前の領域のパーツ点滅アニメーションを停止
                _partsActiveSequence?.Kill();

                // 現在の領域のパーツを点滅させるシーケンスを作成
                _partsActiveSequence = DOTween.Sequence();
                foreach (var image in _partsImageLists[i])
                {
                    image.color = _defaultColor;
                    _partsActiveSequence.Join(image.DOColor(_activeColor, 0.5f));
                }
                _partsActiveSequence.SetLoops(-1, LoopType.Yoyo);

                for (var j = 0; j < playerCount; j++)
                {
                    if (i == j) continue;
                    bgList.Add(_backgroundCanvasGroups[j]);
                }

                foreach (var bg in bgList)
                {
                    fadeInTasks.Enqueue(bg.DOFade(1f, 0.5f).ToUniTask());
                }
                fadeInTasks.Enqueue(_descriptionPanelCanvasGroups[i].DOFade(1f, 0.5f).ToUniTask());

                // フェードイン
                await UniTask.WhenAll(fadeInTasks);

                // 説明表示している間待機
                await UniTask.Delay(3000);

                foreach (var bg in bgList)
                {
                    fadeOutTasks.Enqueue(bg.DOFade(0f, 0.5f).ToUniTask());
                }
                fadeOutTasks.Enqueue(_descriptionPanelCanvasGroups[i].DOFade(0f, 0.5f).ToUniTask());

                // フェードアウト
                await UniTask.WhenAll(fadeOutTasks);

                // パーツ点滅アニメーションを停止
                _partsActiveSequence?.Kill();
                _leftHandImage.color = _defaultColor;
                _rightHandImage.color = _defaultColor;
                _footImage.color = _defaultColor;

                bgList.Clear();
                fadeInTasks.Clear();
                fadeOutTasks.Clear();
            }
            
            await HideAsync();
        }
    }
}