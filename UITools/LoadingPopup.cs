using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using TMPro;

public class LoadingPopup : MonoBehaviour
{
    private const string PrefabPath = "Prefabs/Misc/LoadingPopup"; 
    private TextMeshProUGUI text;
    private float progress = -1;

    public static List<LoadingPopup> Current = new List<LoadingPopup>();

    private void Awake()
    {
    }

    public static LoadingPopup Open()
    {
        var obj = Resources.Load(PrefabPath);
        if (obj == null) throw new Exception("prefab not found");

        var lp = ((GameObject)Instantiate(obj)).GetComponent<LoadingPopup>();
     
        Current.Add(lp);
        lp.Bind();
        return lp;
    }

    void Bind()
    {
        text = gameObject.FirstOrDefault<TextMeshProUGUI>("messageText");
    }

    public void Close()
    {
        Current.Remove(this);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (progress == -1)
        {
            //todo: this            
        }
    }

    /// <summary>
    /// set the progress from 0-1. -1 is unknown and will spin
    /// </summary>
    /// <param name="p"></param>
    public void SetProgress(float p)
    {
        progress = p;
    }
    
    public  void SetMessage(string s)
    {
        text.text = s;
    }
    
}

