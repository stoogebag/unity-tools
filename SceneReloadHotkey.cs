using UnityEngine;
using UnityEngine.InputSystem;

public class SceneReloadHotkey : MonoBehaviour
{
    
    [SerializeField] KeyCode hotkey = KeyCode.R;

    [SerializeField] private bool DestroyPersistentObjects = false;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(hotkey) )
        {
            
            ReloadCurrentScene(DestroyPersistentObjects);
        }   
        if (Input.GetKeyDown(KeyCode.B) )
        {
            if (DestroyPersistentObjects)
            {
                var temp = new GameObject();
                DontDestroyOnLoad(temp);
                var persistentScene = temp.scene;
                DestroyImmediate(temp);

                foreach (var root in persistentScene.GetRootGameObjects())
                {
                    DestroyImmediate(root);
                }
            }
             
            UnityEngine.SceneManagement.SceneManager.LoadScene("GeekMarketInfoScene");
        }   
    }

    public static void ReloadCurrentScene(bool destroyPersistentObjects)
    {
        
        if (destroyPersistentObjects)
        {
            var temp = new GameObject();
            DontDestroyOnLoad(temp);
            var persistentScene = temp.scene;
            DestroyImmediate(temp);

            foreach (var root in persistentScene.GetRootGameObjects())
            {
                DestroyImmediate(root);
            }
        }
             
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        
    }
}
