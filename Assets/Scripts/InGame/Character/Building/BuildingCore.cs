using System;
using InGame.Message;
using InGame.System;
using MessagePipe;
using UniRx;
using UnityEngine;
using VContainer;

namespace InGame.Character
{
    public class BuildingCore : CharacterCore
    {
        [SerializeField] private CharacterParams _params;
        [SerializeField] private HitPointPresenter _hpPresenter;
        [SerializeField] private DamageCollider _damageCollider;

        private DamageObserver _damageObserver;
        private IPublisher<BuildingDestroyedMessage> _buildingDestroyedPublisher;

        public AreaId AreaId { get; private set; }

        [Inject]
        public void Construct(IPublisher<BuildingDestroyedMessage> buildingDestroyedPublisher)
        {
            _buildingDestroyedPublisher = buildingDestroyedPublisher;
        }

        public void SetAreaId(AreaId areaId)
        {
            AreaId = areaId;
        }
        
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

            _hpPresenter.OnHpDecreased
                .Subscribe(OnDead)
                .AddTo(this);
        }
        
        private void OnTakeDamage(AttackParam param)
        {
            _hpPresenter.DecreaseHp(param.AttackPoint.RandomValue);
            HitEffectManager.Instance.Play(param.AttackType, param.HitPosition);
        }

        private void OnDead(Unit unit)
        {
            _buildingDestroyedPublisher.Publish(new BuildingDestroyedMessage(AreaId, this));
            gameObject.SetActive(false);
        }
    }
}