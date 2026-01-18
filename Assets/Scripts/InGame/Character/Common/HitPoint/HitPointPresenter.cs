using UnityEngine;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using InGame.System.UI;

namespace InGame.Character
{
    public class HitPointPresenter : MonoBehaviour
    {
        private HitPointView _view;
        private HitPointModel _model;

        public int CurrentHp => _model.CurrentHp;
        public IObservable<Unit> OnHpDecreased => _model.OnHpDecreased;

        public void Initialize(HitPointView view, CharacterParams characterParams)
        {
            _view = view;
            _model = new HitPointModel(characterParams.MaxHp);
            
            Bind();
        }

        private void Bind()
        {
            if (_view == null) return;
            
            _model.HpRate
                .Where(_ => _view != null)
                .Subscribe(_view.UpdateHp)
                .AddTo(this);
        }

        public void DecreaseHp(int value)
        {
            _model.Decrease(value);
            Debug.Log($"<color=green>[{gameObject.name}] CurrentHp: {_model.CurrentHp}</color>");
        }

        public void Show() => _view.Show();
        public UniTask ShowAsync(IVisibilityContext context = null) => _view.ShowAsync(context);
        public void Hide() => _view.Hide();
        public UniTask HideAsync(IVisibilityContext context = null) => _view.HideAsync(context);
    }
}
