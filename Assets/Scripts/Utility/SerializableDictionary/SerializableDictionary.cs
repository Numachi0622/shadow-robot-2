// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Utility.SerializableDictionary
// {
//     [Serializable]
//     public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
//     {
//         [Serializable]
//         public struct Pair
//         {
//             public TKey Key;
//             public TValue Value;
//             public Pair(TKey key, TValue value)
//             {
//                 Key = key;
//                 Value = value;
//             }
//         }
//     
//         [SerializeField] protected List<Pair> _elements = new List<Pair>();
//     
//         public virtual TKey DefaultKey => default;
//
//         public void OnBeforeSerialize()
//         {
//             _elements.Clear();
//             foreach (var pair in this)
//             {
//                 _elements.Add(new Pair(pair.Key, pair.Value));
//             }
//         }
//
//         public void OnAfterDeserialize()
//         {
//             Clear();
//             foreach (var element in _elements)
//             {
//                 var key = ContainsKey(element.Key) ? DefaultKey : element.Key;
//                 this[key] = element.Value;
//             }
//         }
//     }
//
//     [Serializable]
//     public class SerializableDictionary<TValue> : SerializableDictionary<string, TValue>
//     {
//         public override string DefaultKey => string.Empty;
//     }
// }