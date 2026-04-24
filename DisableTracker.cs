using UnityEngine;

[ExecuteInEditMode] 
public class DeepTracker : MonoBehaviour
{
    // This fires even in the editor and at the absolute earliest moment of Play
    void OnValidate()
    {
        if (Application.isPlaying && !gameObject.activeSelf)
        {
            Debug.Log($"<color=cyan>DEEP TRACE: {gameObject.name} disabled.</color>\n{System.Environment.StackTrace}");
        }
    }

    void Awake()
    {
        Debug.Log($"{gameObject.name} Awake state: {gameObject.activeSelf}");
    }
}