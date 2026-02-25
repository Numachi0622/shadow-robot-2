using System;
using System.Threading;
using UniRx;
using UnityEngine;
using Utility;
using VContainer;

namespace InGame.System.UI
{
    public class TitlePresenter : MonoBehaviour
    {
        [SerializeField] private TitleView _view;
        private TextureRegistry _textureRegistry;
        private readonly IntReactiveProperty _selectTextureIndex = new();

        private readonly string[] InitLoadTextures = 
        {
            "tex_sample_red_01",
            "tex_sample_red_02",
            "tex_sample_blue_01",
            "tex_sample_blue_02",
            "tex_sample_green_01",
            "tex_sample_green_02",
        };
        
        private const string Path = "Assets/Texture/Player";

        [Inject]
        public void Construct(TextureRegistry textureRegistry)
        {
            _textureRegistry = textureRegistry;
        }

        private void Start()
        {
            Bind();
            
            // テクスチャの初期ロード
            InitLoad();
        }

        private void Bind()
        {
            // View => Model
            _view.OnClickRegisterButton.Subscribe(OnRegisterTexture).AddTo(this);

            _view.OnClickSelectTextureButton.Subscribe(idx => _selectTextureIndex.Value = idx).AddTo(this);
            
            // Presenter => View
            _selectTextureIndex.Subscribe(_view.UpdateSelectState).AddTo(this);
            
            // TextureRegistry => View
            _textureRegistry.OnPlayerTextureChanged
                .Subscribe(x => _view.UpdateTextureName(x.playerId, x.context))
                .AddTo(this);
        }

        private async void InitLoad()
        {
            try
            {
                for (var i = 0; i < InitLoadTextures.Length; i++)
                {
                    var texture = await TextureFileLoader.LoadAsync(Path, InitLoadTextures[i], CancellationToken.None);
                    if (texture == null) continue;
                    _textureRegistry.Register(i, texture);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private async void OnRegisterTexture(string fileName)
        {
            try
            {
                var texture = await TextureFileLoader.LoadAsync(Path, fileName, CancellationToken.None);
                if (texture == null) return;

                _textureRegistry.Register(_selectTextureIndex.Value, texture);
                
                // インデックス更新
                _selectTextureIndex.Value = (_selectTextureIndex.Value + 1) % GameConst.MaxTextureCount;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

    }
}