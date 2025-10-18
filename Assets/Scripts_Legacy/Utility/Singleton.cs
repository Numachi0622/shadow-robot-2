using System;
using Interface;
using UnityEngine;
using Utility;

namespace Utility
{
    public class Singleton<T> : MonoBehaviour , ISingleton where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance => _instance;

        public virtual void Initialize()
        {
            if (_instance == null)
            {
                _instance = this as T;
                //DontDestroyOnLoad(gameObject);
                //return;
            }
            
            //Destroy(this);        
        }
    }
}