using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace InGame.Event
{
    public class CombineCutSceneContext : ICutSceneContext
    {
        public PlayableDirector Director { get; set; }
    }
    
    public class CombineCutScene : ICutScene
    {
        private PlayableDirector _director;
        private UniTaskCompletionSource _tcs;
        
        public void Load(ICutSceneContext cutSceneContext)
        {
            if (cutSceneContext is not CombineCutSceneContext context) return;
            _director = Object.Instantiate(context.Director);
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            if (_director == null)
            {
                Debug.LogError("[CombineCutScene] PlayAsync called before Load or Director is null.");
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