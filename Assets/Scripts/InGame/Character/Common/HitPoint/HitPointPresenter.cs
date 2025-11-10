using UnityEngine;
using UniRx;
using System;

namespace InGame.Character
{
    public class HitPointPresenter : MonoBehaviour
    {
        [SerializeField] private HitPointView _view;
        private HitPointModel _model;
        
        public int CurrentHp => _model.Hp.Value;
        public IObservable<Unit> OnHpDecreased => _model.OnHpDecreased;

        public void Initialize(CharacterParams characterParams)
        {
            _model = new HitPointModel(characterParams.MaxHp);

            var currentHp = _model.Hp.Value;
            _view?.Initialize(currentHp, characterParams.MaxHp);

            Bind();
        }

        private void Bind()
        {
            _model.Hp
                .Where(_ => _view != null)
                .Subscribe(_view.UpdateHp)
                .AddTo(this);
        }

        public void DecreaseHp(int value)
        {
            _model.Decrease(value);
        }
    }
}
