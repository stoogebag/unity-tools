#if FISHNET
using System;
using System.Net;
using System.Net.Sockets;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UniRx;

namespace stoogebag
{
    public static class NetworkingExtensions
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


    }
    public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour {

        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }

        protected virtual void OnApplicationQuit() {
            Instance = null;
            Destroy(gameObject);
        }
    }
    //
    // public abstract class PersistentSingleton<T> : Singleton<T> where T : NetworkBehaviour
    // {
    //     protected override void Awake()
    //     {
    //         base.Awake();
    //         DontDestroyOnLoad(gameObject);
    //     }
    // }

}
#endif