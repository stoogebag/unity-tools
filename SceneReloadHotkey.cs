using UnityEngine;
using UnityEngine.InputSystem;

public class SceneReloadHotkey : MonoBehaviour
{
    
    [SerializeField] KeyCode hotkey = KeyCode.R;
    
    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown(hotkey) )
         {
             UnityEngine.SceneManagement.SceneManager.LoadScene(
                 UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }   
    }
}
