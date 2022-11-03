using TMPro;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.DebugTools
{
    public class DebugWindow : MonoBehaviour
    {

        public KeyCode ShowHideKey = KeyCode.Tilde;
        private Canvas debugCanvas;
        private TextMeshProUGUI text;

        //public static string prefabPath;

        private void Start()
        {
            text = GetComponentInChildren<TextMeshProUGUI>(true);
            debugCanvas = GetComponentInChildren<Canvas>(true);
        
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(ShowHideKey))
            {
                debugCanvas.gameObject.SetActive(!debugCanvas.gameObject.activeSelf);
            }
        
            text.text = GetText();
        }

        private string GetText()
        {
            var fps = 1f / Time.unscaledDeltaTime;
            var width = Display.main.renderingWidth;
            var height = Display.main.renderingHeight;


            return $"{fps} fps \n {width}x{height}";
        }
    }
}
