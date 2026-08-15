#if UNITASK
#if ODIN_INSPECTOR
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using stoogebag;
using stoogebag.Utils;

namespace stoogebag.UITools.Windows
{
    public class WindowManager : Singleton<WindowManager>
    {
        private Dictionary<string,Window> _windows = new Dictionary<string, Window>();

        [SerializeField]
        private List<Window> _windowsList = new List<Window>();
        
        // tests
        [Button]
        void OpenWindowTest()
        {
            var yn = GetWindow("YesNo") as YesNoWindow;
            yn.Bind(()=> {print("yeay");});
         
            Open("YesNo");
        }
    
        [Button] void CloseWindowTest()
        {
            Close("Panel");
        }

        private void Start()
        {
            // foreach (var w in Resources.FindObjectsOfTypeAll<Window>()) 
            // {
            //     Register(w);
            // }
            
            foreach (var w in _windowsList)//now im using subwindows, ill manually add
            {
                Register(w);
            }
        }

        public static void Register(Window w)
        {
            if (Instance._windows.ContainsKey(w.name))
            {
                Debug.LogWarning($"WindowManager: window '{w.name}' already registered, replacing.");
            }
            Instance._windows[w.name] = w;
        }

        public static void Deregister(string name)
        {
            Instance._windows.Remove(name);
        }

        public static void Deregister(Window w)
        {
            Instance._windows.Remove(w.name);
        }

        public static async UniTask Open(string windowName, bool exclusive = false)
        {
            if (exclusive) CloseAll();
            var w = GetWindow(windowName);
            if (w != null) await w.Activate();
        }
        public static async UniTask Close(string windowName)
        {
            var w = GetWindow(windowName);
            if (w != null) await w.Deactivate();
        }

        public static Window GetWindow(string name)
        {
            if (Instance._windows.TryGetValue(name, out var w)) return w;
            Debug.LogWarning($"WindowManager: no window named '{name}' registered.");
            return null;
        }

        public static async UniTask CloseAll()
        {
            var tasks = Enumerable.Select(Instance._windows.Values, t => t.Deactivate()).ToArray();
            await UniTask.WhenAll(tasks);
        }
    }
}


#endif
#endif