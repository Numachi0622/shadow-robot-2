using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct Pair
        {
            public TKey Key;
            public TValue Value;
            public Pair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    
        [SerializeField] protected List<Pair> _elements = new List<Pair>();
        
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Clear();
            foreach (var element in _elements)
            {
                this[element.Key] = element.Value;
            }
        }
    }
}