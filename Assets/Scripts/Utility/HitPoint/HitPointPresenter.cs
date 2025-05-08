using UnityEngine;
using UniRx;
using System;

public class HitPointPresenter : MonoBehaviour
{
    [SerializeField] private HitPointView _view;
    private HitPointModel _model;
    
    public void Initialize(CharacterParams characterParams)
    {
        _model = new HitPointModel(characterParams.MaxHp);
        _view.Initialize();
        
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
        _model.OnHpDeleted = onHpDecreased;
    }
}
