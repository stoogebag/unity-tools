using UnityEngine;

namespace stoogebag._2dConvos
{
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager instance;

        public VIDE_Assign OpeningDialogue;
        public VIDE_Assign HelpDialogue;

        public VIDE_Assign FailDialogue;
        public VIDE_Assign SuccessDialogue;
    
        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            ui = FindObjectOfType<Template_UIManager>();
        
            if(OpeningDialogue !=null) StartConvo(OpeningDialogue);
        
        }

        // Update is called once per frame

        public void StartConvo(VIDE_Assign assign)
        {
            ui.Interact(assign);
        }

        void Update()
        {
            ui.gameObject.SetActive(true);
            ui.enabled = true;
        }

        private Template_UIManager ui;


        public void End()
        {
            // Camera.main.GetComponentInChildren<CameraFade>().FadeOut(Color.white, () =>
            //     {
            //         SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
            //     });
        }

    }
}
