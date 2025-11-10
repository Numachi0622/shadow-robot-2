using System;
using UniRx;
using UnityEngine;

namespace InGame.Character.Building
{
    public class BuildingCore : CharacterCore
    {
        [SerializeField] private CharacterParams _params;
        [SerializeField] private HitPointPresenter _hpPresenter;
        [SerializeField] private DamageCollider _damageCollider;

        private DamageObserver _damageObserver;
        
        public override void Initialize()
        {
            _damager = new Damager(_hpPresenter);
            _damageObserver = new DamageObserver();
            _damageCollider.Initialize(_damageObserver);
            _hpPresenter.Initialize(_params);
            
            Bind();
        }
        
        private void Bind()
        {
            _damageObserver.OnTakeDamage
                .Subscribe(OnTakeDamage)
                .AddTo(this);
        }
        
        private void OnTakeDamage(AttackParam param)
        {
            Debug.Log($"[BuildingCore] OnTakeDamage : {param.AttackPoint.RandomValue}");
            _hpPresenter.DecreaseHp(param.AttackPoint.RandomValue);
        }
    }
}