using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Instance._windows.Add(w.name,w);
        }

        public static void Deregister(string name)
        {
            Instance._windows.Remove(name);
        }

        public static async UniTask Open(string windowName, bool exclusive = false)
        {
            if (exclusive) CloseAll();
            await GetWindow(windowName).Activate();
        }
        public static async UniTask Close(string windowName)
        {
            await GetWindow(windowName).Deactivate();
        }

        public static Window GetWindow(string name) => Instance._windows[name];

        public static async UniTask CloseAll()
        {
            var tasks = Enumerable.Select(Instance._windows.Values, t => t.Deactivate()).ToArray();
            await UniTask.WhenAll(tasks);
        }
    }
}


