using Cysharp.Threading.Tasks;

namespace InGame.System.UI
{
    public interface IVisibilityController
    {
        bool IsActive { get; }
        void Show();
        void Hide();
        UniTask ShowAsync(IVisibilityContext context = null);
        UniTask HideAsync(IVisibilityContext context = null);
    }

    public interface IVisibilityContext
    {
    }
}