using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;
using UnityEngine;

namespace InGame.Event
{
    public class BossEnterCutSceneContext : ICutSceneContext
    {
        public PlayableDirector Director { get; set; }
        public int BuildingCount { get; set; }
    }
    
    public class BossEnterCutScene : ICutScene
    {
        private PlayableDirector _director;
        private UniTaskCompletionSource _tcs;
        
        public void Load(ICutSceneContext cutSceneContext)
        {
            if (cutSceneContext is not BossEnterCutSceneContext context) return;
            
            _director = Object.Instantiate(context.Director);
            _director.GetComponent<BossEnterCutSceneController>()?.Initialize(context.BuildingCount);
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            if (_director == null)
            {
                Debug.LogError("[BossEnterCutScene] PlayAsync called before Load or Director is null.");
                return;
            }
            
            _tcs = new UniTaskCompletionSource();
            _director.stopped += OnStopped;
            
            using var reg = ct.Register(() => _tcs.TrySetCanceled());

            try
            {
                _director.Play();
                await _tcs.Task;
            }
            finally
            {
                _director.stopped -= OnStopped;
                Object.Destroy(_director.gameObject);
            }
        }
        
        private void OnStopped(PlayableDirector _) => _tcs.TrySetResult();
    }
}