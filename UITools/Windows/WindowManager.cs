using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.UITools.Windows
{
    public class WindowManager : Singleton<WindowManager>
    {
        private Dictionary<string,Window> _windows = new Dictionary<string, Window>();

        // tests
        [Button]
        public void OpenWindow()
        {
            var yn = GetWindow("YesNo") as YesNoWindow;
            yn.Bind(()=> {print("yeay");});
         
            Open("YesNo");
        }
    
        [Button] public void CloseWindow()
        {
            Close("Panel");
        }

        private void Start()
        {
            foreach (var w in Resources.FindObjectsOfTypeAll<Window>())
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

        public static async Task Open(string windowName, bool exclusive = false)
        {
            if (exclusive) CloseAll();
            await GetWindow(windowName).Activate();
        }
        public static async Task Close(string windowName)
        {
            await GetWindow(windowName).Deactivate();
        }

        public static Window GetWindow(string name) => Instance._windows[name];

        public static async Task CloseAll()
        {
            var tasks = Instance._windows.Values.Select(t => t.Deactivate()).ToArray();
            await Task.WhenAll(tasks);
        }
    }
}


