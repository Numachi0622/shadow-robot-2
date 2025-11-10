using UnityEngine;

namespace InGame.Character
{
    public class Damager : IDamageable
    {
        private readonly HitPointPresenter _hpPresenter;
        
        public Damager(HitPointPresenter hpPresenter)
        {
            _hpPresenter = hpPresenter;
        }
        
        public void Damage(int damage)
        {
            _hpPresenter.DecreaseHp(damage);
        }
    }
}