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

        public void Initialize(CharacterParams characterParams)
        {
            _model = new HitPointModel(characterParams.MaxHp);

            var currentHp = _model.Hp.Value;
            _view.Initialize(currentHp, characterParams.MaxHp);

            Bind(characterParams);
        }

        public void Initialize(CharacterParams characterParams, HitPointView view)
        {
            if (_view == null)
            {
                _view = view;
            }

            Initialize(characterParams);
        }

        private void Bind(CharacterParams characterParams)
        {
            _model.Hp
                .Subscribe(hp => _view.UpdateHp(hp, characterParams.MaxHp))
                .AddTo(this);
        }

        public void DecreaseHp(int value)
        {
            _model.Decrease(value);
        }

        public void OnHpDeleted(Action onHpDeleted)
        {
            _model.OnHpDeleted = onHpDeleted;
        }

        public void OnHpDecreased(Action onHpDecreased)
        {
            _model.OnHpDecreased = onHpDecreased;
        }
    }
}
