using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/SoundData")]
    public class SoundData : ScriptableObject
    {
        public Sound[] Sounds;
    }

    [System.Serializable]
    public struct Sound
    {
        [SerializeField] private AudioClip[] _clips;
        public AudioClip Get => _clips.Length == 1 ? _clips[0] : _clips[Random.Range(0, _clips.Length)];
    }
}
