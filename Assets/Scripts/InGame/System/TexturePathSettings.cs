using System;
using System.IO;
using UnityEngine;

namespace InGame.System
{
    /// <summary>
    /// テクスチャのパス設定を管理するクラス
    /// </summary>
    [Serializable]
    public class TexturePathConfig
    {
        public string texturePath = "";
    }

    public static class TexturePathSettings
    {
        private const string ConfigFileName = "texture_config.json";
        private const string EditorDefaultPath = "Assets/Texture/Player";

        private static TexturePathConfig _config;
        private static bool _isInitialized;

        /// <summary>
        /// テクスチャのパスを取得
        /// エディタではデフォルトパス、ビルド後はJSONから読み込み
        /// </summary>
        public static string GetTexturePath()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

#if UNITY_EDITOR
            // エディタ上ではデフォルトのAssetsパスを使用
            return EditorDefaultPath;
#else
            // ビルド後はJSONから読み込んだパスを使用
            return _config?.texturePath ?? GetDefaultBuildPath();
#endif
        }

        private static void Initialize()
        {
            _isInitialized = true;

#if !UNITY_EDITOR
            LoadConfig();
#endif
        }

        private static void LoadConfig()
        {
            try
            {
                var configPath = GetConfigFilePath();

                if (File.Exists(configPath))
                {
                    var json = File.ReadAllText(configPath);
                    _config = JsonUtility.FromJson<TexturePathConfig>(json);
                    Debug.Log($"Texture config loaded from: {configPath}");
                }
                else
                {
                    // 設定ファイルが存在しない場合はデフォルト設定を作成
                    CreateDefaultConfig();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load texture config: {e.Message}");
                _config = new TexturePathConfig { texturePath = GetDefaultBuildPath() };
            }
        }

        private static void CreateDefaultConfig()
        {
            try
            {
                _config = new TexturePathConfig { texturePath = GetDefaultBuildPath() };
                var configPath = GetConfigFilePath();
                var json = JsonUtility.ToJson(_config, true);
                File.WriteAllText(configPath, json);
                Debug.Log($"Default texture config created at: {configPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create default texture config: {e.Message}");
            }
        }

        private static string GetConfigFilePath()
        {
            // 実行ファイルと同じディレクトリに設定ファイルを配置
            return Path.Combine(Application.dataPath, "..", ConfigFileName);
        }

        private static string GetDefaultBuildPath()
        {
            // ビルド後のデフォルトパス（実行ファイルと同階層のTexturesフォルダ）
            return Path.Combine(Application.dataPath, "..", "Textures", "Player");
        }

#if UNITY_EDITOR
        /// <summary>
        /// エディタ用：サンプル設定ファイルを生成（ビルド前に確認用）
        /// </summary>
        [UnityEditor.MenuItem("Tools/Generate Sample Texture Config")]
        public static void GenerateSampleConfig()
        {
            var sampleConfig = new TexturePathConfig
            {
                texturePath = Path.Combine(Application.dataPath, "..", "Textures", "Player")
            };

            var configPath = GetConfigFilePath();
            var json = JsonUtility.ToJson(sampleConfig, true);
            File.WriteAllText(configPath, json);

            Debug.Log($"Sample texture config generated at: {configPath}\n{json}");
            UnityEditor.EditorUtility.RevealInFinder(configPath);
        }
#endif
    }
}
