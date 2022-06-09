using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using TMPro;

public class DebugWindow : MonoBehaviour
{

    public KeyCode ShowHideKey = KeyCode.Tilde;
    private Canvas debugCanvas;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>(true);
        debugCanvas = GetComponentInChildren<Canvas>(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(ShowHideKey))
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
