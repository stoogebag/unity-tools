using UnityEngine;

namespace stoogebag.Utils
{


    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        public static T Instance { get; private set; }

        public bool Persistent;

        protected virtual void Awake()
        {
            if(Persistent) DontDestroyOnLoad(gameObject);
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }

    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }


}