using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Exception = System.Exception;

namespace InGame.System
{
    public class TextureFileLoader
    {
        private static readonly string[] SupportedExtensions = { ".png", ".jpg" };

        /// <summary>
        /// 指定ディレクトリからファイル名で画像を検索し Texture2D を返す。
        /// 見つからない・ロード失敗時は null を返す。
        /// </summary>
        public static async UniTask<Texture2D> LoadAsync(string directoryPath, string fileName, CancellationToken ct)
        {
            var filePath = FindFilePath(directoryPath, fileName);
            if (filePath == null)
            {
                Debug.LogWarning($"[TextureFileLoader] ファイルが見つかりませんでした: {directoryPath}/{fileName}");
                return null;
            }

            var bytes = await ReadBytesAsync(filePath, ct);
            if (bytes == null) return null;

            // 一旦2x2で生成し、LoadImageでサイズを自動調整させる
            var texture = new Texture2D(2, 2);
            if (!texture.LoadImage(bytes))
            {
                Debug.LogWarning($"[TextureFileLoader] テクスチャのロードに失敗しました: {filePath}");
                return null;
            }

            texture.name = fileName;
            return texture;
        }

        /// <summary>
        /// 対応拡張子を順に試してファイルパスを返す。見つからなければ null。
        /// </summary>
        private static string FindFilePath(string directoryPath, string fileName)
        {
            foreach (var ext in SupportedExtensions)
            {
                var path = Path.Combine(directoryPath, fileName + ext);
                if (File.Exists(path)) return path;
            }
            return null;
        }

        /// <summary>
        /// ファイルの読み込みをスレッドプールで非同期実行する。
        /// </summary>
        private static async UniTask<byte[]> ReadBytesAsync(string filePath, CancellationToken ct)
        {
            try
            {
                var bytes = await UniTask.RunOnThreadPool(
                    () => File.ReadAllBytes(filePath),
                    cancellationToken: ct
                );
                return bytes;
            }
            catch (Exception e)
            {
                Debug.LogError($"[TextureFileLoader] ファイル読み込みエラー: {filePath}\n{e}");
                return null;
            }
        }
    }
}
