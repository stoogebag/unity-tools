using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace stoogebag.Common
{
    public class FpsCounter :MonoBehaviour
    {
        private TextMeshProUGUI uiText;
    
    
        string display = "{0} FPS";
        public float fps;

        private void Awake()
        {
            uiText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            InvokeRepeating(nameof(GetFPS), 1,1);
        }

        void GetFPS()
        {
            fps = 1f / Time.unscaledDeltaTime;
            uiText.text = string.Format(display,fps.ToString()); 
            
            
        }
    
    }
}