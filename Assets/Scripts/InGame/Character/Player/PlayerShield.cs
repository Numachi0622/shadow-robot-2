using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerShield : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _shieldEffect;
        [SerializeField] private Collider _shieldCollider;

        public async UniTask OpenShieldAsync()
        {
            _shieldEffect.Play();
            _shieldCollider.enabled = true;

            await UniTask.WaitUntil(() => !_shieldEffect.isPlaying);
            _shieldCollider.enabled = false;
        }
    }
}