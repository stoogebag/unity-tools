using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        assign = FindObjectOfType<VIDE_Assign>();
        ui = FindObjectOfType<Template_UIManager>();
    }

    // Update is called once per frame

    public void StartConvo()
    {
        ui.Interact(assign);

        
    }

    void Update()
    {
        ui.gameObject.SetActive(true);
        ui.enabled = true;
    }

    private VIDE_Assign assign;
    private Template_UIManager ui;


    public void End()
    {
        // Camera.main.GetComponentInChildren<CameraFade>().FadeOut(Color.white, () =>
        //     {
        //         SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
        //     });
    }

}
