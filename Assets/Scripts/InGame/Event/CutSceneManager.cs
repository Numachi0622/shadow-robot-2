using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace InGame.Event
{
    public class CutSceneManager
    {
        private static readonly Dictionary<Type, ICutScene> _cutScenes = new();
        
        public static async UniTask LoadAndPlayAsync<T>(ICutSceneContext context, CancellationToken token) where T : ICutScene, new()
        {
            if (_cutScenes.TryGetValue(typeof(T), out var cutScene))
            {
                cutScene.Load(context);
                await cutScene.PlayAsync(token);
                return;
            }

            var instance = new T();
            _cutScenes.Add(typeof(T), instance);
            instance.Load(context);
            await instance.PlayAsync(token);
        }
    }
}