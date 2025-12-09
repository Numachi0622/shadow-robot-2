using InGame.Character;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace InGame.System
{
    public class CharacterFactory
    {
        private readonly IObjectResolver _container;
        
        [Inject]
        public CharacterFactory(IObjectResolver container)
        {
            _container = container;
        }
        
        public CharacterCore Create<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : CharacterCore
        {
            var prefab = _container.Resolve<T>();
            var instance = _container.Instantiate(prefab, position, rotation, parent);
            return instance;
        }

        public CharacterCore Create(CharacterCore prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var instance = _container.Instantiate(prefab, position, rotation, parent);
            return instance;
        }
    }
}