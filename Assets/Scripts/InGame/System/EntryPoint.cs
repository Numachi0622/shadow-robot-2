using InGame.Character;
using Utility;
using TNRD;
using UnityEngine;

namespace InGame.System
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<ISingleton>[] _singletons;

        [SerializeField] private PlayerManager _playerManager;

        private void Awake()
        {
            foreach (var singleton in _singletons)
            {
                singleton.Value.Initialize();
            }
            
            _playerManager.Initialize();
        }
        
        private void Update()
        {
            _playerManager.OnUpdate();
        }
    }
}