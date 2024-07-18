using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ControllerDetect : MonoBehaviour
{
    private bool connected = false;

    [Button]
    IEnumerator CheckForControllers() {
        while (true) {
            var controllers = Input.GetJoystickNames();

            if (!connected && controllers.Length > 0) {
                connected = true;
                Debug.Log("Connected " + string.Join(';',controllers));
            
            } else if (connected && controllers.Length == 0) {         
                connected = false;
                Debug.Log("Disconnected");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void Awake() {
        StartCoroutine(CheckForControllers());
    }
}