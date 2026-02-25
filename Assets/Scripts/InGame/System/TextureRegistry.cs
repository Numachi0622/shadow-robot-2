using System;
using System.Collections.Generic;
using InGame.Character;
using UniRx;
using UnityEngine;
using Utility;
using Object = UnityEngine.Object;

namespace InGame.System
{
    public struct PlayerTextureContext
    {
        public Texture2D Texture1 { get; set; }
        public Texture2D Texture2 { get; set; }
    }

    public class TextureRegistry
    {
        private readonly ReactiveCollection<Texture2D> _textures = new();

        public IReadOnlyReactiveCollection<Texture2D> Textures => _textures;

        public PlayerTextureContext TextureContext(CharacterId id)
        {
            var playerId = id.Value;
            var texture1Index = playerId * 2;
            var texture2Index = playerId * 2 + 1;

            var texture1 = texture1Index < _textures.Count ? _textures[texture1Index] : null;
            var texture2 = texture2Index < _textures.Count ? _textures[texture2Index] : null;

            return new PlayerTextureContext
            {
                Texture1 = texture1,
                Texture2 = texture2
            };
        }

        public IReadOnlyList<PlayerTextureContext> GetAllTextureContexts()
        {
            var contexts = new List<PlayerTextureContext>();

            for (int playerId = 0; playerId < GameConst.MaxPlayerCount; playerId++)
            {
                var texture1Index = playerId * 2;
                var texture2Index = playerId * 2 + 1;

                var texture1 = texture1Index < _textures.Count ? _textures[texture1Index] : null;
                var texture2 = texture2Index < _textures.Count ? _textures[texture2Index] : null;

                contexts.Add(new PlayerTextureContext
                {
                    Texture1 = texture1,
                    Texture2 = texture2
                });
            }

            return contexts;
        }
        
        /// <summary>
        /// テクスチャが変更されたときに、そのテクスチャが属するplayerIdとペアのテクスチャを発行する
        /// </summary>
        public IObservable<(int playerId, PlayerTextureContext context)> OnPlayerTextureChanged =>
            _textures.ObserveReplace().Select(x => x.Index).Merge(_textures.ObserveAdd().Select(x => x.Index)
            )
            .Select(index =>
            {
                var playerId = index / 2;
                var texture1Index = playerId * 2;
                var texture2Index = playerId * 2 + 1;

                var texture1 = texture1Index < _textures.Count ? _textures[texture1Index] : null;
                var texture2 = texture2Index < _textures.Count ? _textures[texture2Index] : null;

                var context = new PlayerTextureContext
                {
                    Texture1 = texture1,
                    Texture2 = texture2
                };

                return (playerId, context);
            });

        /// <summary>
        /// 2人のプレイヤーのテクスチャペアを入れ替える
        /// </summary>
        public void SwapPlayerTextures(int playerIdA, int playerIdB)
        {
            var indexA1 = playerIdA * 2;
            var indexA2 = playerIdA * 2 + 1;
            var indexB1 = playerIdB * 2;
            var indexB2 = playerIdB * 2 + 1;

            // 範囲外の場合は拡張
            var maxIndex = Mathf.Max(indexA1, indexA2, indexB1, indexB2);
            while (_textures.Count <= maxIndex)
            {
                _textures.Add(null);
            }

            // 一時変数に保存
            var tempA1 = _textures[indexA1];
            var tempA2 = _textures[indexA2];

            // 明示的に再代入してObserveReplaceを発火させる
            _textures[indexA1] = _textures[indexB1];
            _textures[indexA2] = _textures[indexB2];
            _textures[indexB1] = tempA1;
            _textures[indexB2] = tempA2;
        }

        /// <summary>
        /// テクスチャをインデックスで登録する。インデックスが範囲外の場合は自動的に拡張する。
        /// </summary>
        public void Register(int idx, Texture2D texture)
        {
            if (idx >= GameConst.MaxTextureCount) return;
            
            // インデックスが範囲外の場合は拡張
            while (_textures.Count <= idx)
            {
                _textures.Add(null);
            }

            _textures[idx] = texture;
            
            Debug.Log("[TextureRegistry] Registered texture at index " + idx + ": " + texture.name);
        }

        /// <summary>
        /// 登録済みのすべてのテクスチャを破棄してレジストリを空にする。
        /// </summary>
        public void Clear()
        {
            foreach (var texture in _textures)
            {
                if (texture != null)
                {
                    Object.Destroy(texture);
                }
            }
            _textures.Clear();
        }
    }
}
