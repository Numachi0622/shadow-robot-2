using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using InGame.Character;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace InGame.System
{
    /// <summary>
    /// PoseDataの取得・管理を担当するサービスクラス
    /// </summary>
    public class PoseDataService
    {
        private const string PoseDataLabel = "PoseData";
        private List<PoseData> _cachedPoseData;
        private AsyncOperationHandle<IList<PoseData>> _loadHandle;

        /// <summary>
        /// PoseDataを事前ロード（任意）
        /// </summary>
        public async UniTask PreloadAsync()
        {
            if (_cachedPoseData != null) return;

            _loadHandle = Addressables.LoadAssetsAsync<PoseData>(PoseDataLabel, null);
            var result = await _loadHandle.ToUniTask();
            _cachedPoseData = new List<PoseData>(result);

            if (_cachedPoseData.Count == 0)
            {
                Debug.LogWarning($"[PoseDataService] No PoseData found with label '{PoseDataLabel}'");
            }
        }

        /// <summary>
        /// ランダムなPoseDataを取得
        /// </summary>
        public async UniTask<PoseData> GetRandomPoseAsync()
        {
            // キャッシュがない場合はロード
            if (_cachedPoseData == null)
            {
                await PreloadAsync();
            }

            if (_cachedPoseData == null || _cachedPoseData.Count == 0)
            {
                Debug.LogError("[PoseDataService] No PoseData available");
                return null;
            }

            int randomIndex = Random.Range(0, _cachedPoseData.Count);
            return _cachedPoseData[randomIndex];
        }

        /// <summary>
        /// 特定の条件でPoseDataを取得（将来の拡張用）
        /// </summary>
        public async UniTask<PoseData> GetPoseByNameAsync(string poseName)
        {
            if (_cachedPoseData == null)
            {
                await PreloadAsync();
            }

            return _cachedPoseData?.Find(p => p.poseName == poseName);
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            if (_loadHandle.IsValid())
            {
                Addressables.Release(_loadHandle);
            }
            _cachedPoseData?.Clear();
            _cachedPoseData = null;
        }
    }
}
