using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    private Button NextButton; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        NextButton = gameObject.FirstOrDefault<Button>("NextButton");

        NextButton.OnClickAsObservable().Subscribe(t =>
        {
            FindObjectOfType<SceneLoadManager>()
                .ChangeSceneWithFade(SceneManager.GetActiveScene().buildIndex + 1, 1, Color.black).Forget();
        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
