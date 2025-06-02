using UnityEngine;

namespace Utility
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _seSource;
        [SerializeField] private SoundData _soundData;

        public override void Initialize()
        {
            base.Initialize();
        }

        public void PlayBgm(int id)
        {
            _bgmSource.clip = _soundData.Sounds[id].Get;
            _bgmSource.Play();   
        }

        public void PlaySe(int id)
        {
            _seSource.PlayOneShot(_soundData.Sounds[id].Get);
        }
    }
}