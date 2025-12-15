using UnityEngine;

namespace stoogebag.Utils
{


    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                    if (_instance == null)
                        _instance = FindAnyObjectByType<T>();
                    return _instance;
                
            }
        }

        public bool Persistent;

        protected virtual void Awake()
        {
            if(Persistent) DontDestroyOnLoad(gameObject);
            
            //todo: needed?
            if (FindObjectsByType<T>(FindObjectsSortMode.None).Length > 2)
            {
                print("found two copies of singleton " + typeof(T).ToString() + ", destroying one");
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
        }

        protected virtual void OnApplicationQuit()
        {
            _instance = null;
            //Destroy(gameObject);
        }
        
        private void OnDestroy()
        {
            // Important: Release the static reference when destroyed
            if (_instance == this)
            {
                _instance = null;
            }
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