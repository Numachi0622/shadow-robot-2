using DG.Tweening;
using UnityEngine;

namespace Utility
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _seSource;
        [SerializeField] private SoundData _soundData;
        private Sequence _stopSequence;

        public override void Initialize()
        {
            base.Initialize();
        }

        public void PlayBgm(int id)
        {
            _bgmSource.clip = _soundData.Sounds[id].Get;
            _bgmSource.Play();   
        }

        public void StopBgm()
        {
            var currentVol = _bgmSource.volume;
            _stopSequence?.Kill();
            _stopSequence = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(_bgmSource.DOFade(0f, 1f))
                .AppendCallback(() =>
                {
                    _bgmSource.Stop();
                    _bgmSource.volume = currentVol;
                });
        }

        public void PlaySe(int id)
        {
            _seSource.PlayOneShot(_soundData.Sounds[id].Get);
        }
    }
}