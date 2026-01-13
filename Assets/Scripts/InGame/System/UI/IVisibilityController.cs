using Cysharp.Threading.Tasks;

namespace InGame.System.UI
{
    public interface IVisibilityController
    {
        bool IsActive { get; }
        void Show();
        void Hide();
        UniTask ShowAsync();
        UniTask HideAsync();
    }
}