
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Common;
using stoogebag.Utils;
using stoogebag.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using UnityEngine.Rendering;

//using VInspector;

public class SceneLoadManager : Singleton<SceneLoadManager>
{

    [SerializeField] private SerializedDictionary<string, bool> _scenesToLoad;


    [SerializeField] private bool LoadOnStart = true;
    
    //VInspector.SerializedDictionary<string, bool>
    
    public ReactiveProperty<bool> loading = new ReactiveProperty<bool>(false);

    private void Start()
    {
        if (LoadOnStart) LoadScenes();
    }

    [Sirenix.OdinInspector.Button]
    async UniTask LoadScenes()
    {
        loading.Value = true;
        var tasks = new List<UniTask>();
        foreach (var (key, value) in _scenesToLoad.Where(t=>t.Value))
        {
            if (SceneManager.GetSceneByName(key) != null) continue; //already loaded.
            
             tasks.Add(SceneManager.LoadSceneAsync(key, LoadSceneMode.Additive).ToUniTask());
        }

        await UniTask.WhenAll(tasks);
        loading.Value = false;
        print("all scenes loaded.");
       // SceneManager.SetActiveScene(SceneManager.GetSceneByName("ISLAND"));
    }
    
    
    public async UniTask ChangeScene(string sceneName, Tween anim)
    {
        var tasks = new List<UniTask>();

        var sceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        sceneLoad.allowSceneActivation = false;
        
        tasks.Add(anim.Play().OnComplete(()=> sceneLoad.allowSceneActivation = true).ToUniTask());
        tasks.Add(sceneLoad.ToUniTask());
        
        await UniTask.WhenAll(tasks);
    }
    public async UniTask ChangeScene(int sceneIndex, Tween anim)
    {
        var tasks = new List<UniTask>();

        var sceneLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        sceneLoad.allowSceneActivation = false;
        tasks.Add(anim.Play().OnComplete(()=> sceneLoad.allowSceneActivation = true).ToUniTask());
        tasks.Add(sceneLoad.ToUniTask());
        
        await UniTask.WhenAll(tasks);
    }
    
    public async UniTask ChangeSceneWithFade(string sceneName, float fadeTime, Color color)
    {
        var uiImage = gameObject.FirstOrDefault<Image>("CameraFadePanel");

        if (uiImage == null)
            uiImage = FindObjectsOfType<Image>().FirstOrDefault(t => t.gameObject.name == "CameraFadePanel");
        
        uiImage.enabled = true;
        
        uiImage.color = uiImage.color.WithAlpha(0);
        var tween = uiImage.DOColor(color, fadeTime);
        
        await ChangeScene(sceneName, tween);
    }
    public async UniTask ChangeSceneWithFade(int sceneIndex, float fadeTime, Color color)
    {
        var uiImage = gameObject.FirstOrDefault<Image>("CameraFadePanel");
        
        uiImage.enabled = true;
        
        uiImage.color = uiImage.color.WithAlpha(0);
        var tween = uiImage.DOColor(color, fadeTime);
        
        await ChangeScene(sceneIndex, tween);
    }

    public async UniTask FadeToTransparent(float fadeTime, Color startColor)
    {
        var uiImage = gameObject.FirstOrDefault<Image>("CameraFadePanel");
        uiImage.enabled = true;
        uiImage.color = startColor;
        var tween = uiImage.DOColor( startColor.WithAlpha(0), fadeTime);
        
        await tween.Play().ToUniTask();
    }
        

    [Sirenix.OdinInspector.Button]
    void TestChangeScene()
    {
        ChangeSceneWithFade("level2", 1f, Color.black).Forget();
    }

    protected override void Awake()
    {
        base.Awake();
        
       // SceneManager.activeSceneChanged += (s,t) => FadeToTransparent(1, Color.white).Forget();
    }
}
