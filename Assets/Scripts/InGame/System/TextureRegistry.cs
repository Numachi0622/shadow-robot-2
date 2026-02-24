using System.Collections.Generic;
using UnityEngine;

namespace InGame.System
{
    public class TextureRegistry
    {
        private readonly Dictionary<string, Texture2D> _textures = new();

        /// <summary>
        /// テクスチャを名前で登録する。同名が既に存在する場合は上書きして古いテクスチャを破棄する。
        /// </summary>
        public void Register(string id, Texture2D texture)
        {
            if (_textures.TryGetValue(id, out var existing))
            {
                Object.Destroy(existing);
            }
            _textures[id] = texture;
            
            Debug.Log(_textures.Count);
        }

        /// <summary>
        /// 名前でテクスチャを取得する。見つからない場合は false を返す。
        /// </summary>
        public bool TryGet(string id, out Texture2D texture)
        {
            return _textures.TryGetValue(id, out texture);
        }

        /// <summary>
        /// 登録済みのすべてのテクスチャを破棄してレジストリを空にする。
        /// </summary>
        public void Clear()
        {
            foreach (var texture in _textures.Values)
            {
                Object.Destroy(texture);
            }
            _textures.Clear();
        }
    }
}
