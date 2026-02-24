using System.Threading;
using Cysharp.Threading.Tasks;

namespace InGame.Event
{
    public interface ICutScene
    {
        public void Load(ICutSceneContext context);
        public UniTask PlayAsync(CancellationToken token);
    }
}