using System.Collections.Generic;
using TNRD;
using UnityEngine;
using Utility;

namespace InGame.System
{
    public class SingletonManager : MonoBehaviour
    {
        [SerializeField] private List<SerializableInterface<ISingleton>> _singletons;

        public void Awake()
        {
            foreach (var singleton in _singletons)
            {
                singleton.Value.Initialize();
            }
        }
    }
}