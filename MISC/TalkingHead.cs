using System;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using TMPro;
using UniRx;
using UnityEngine;

public class TalkingHead : Window
{
    private TextMeshProUGUI text;
    private TextMeshProUGUI nameText;
    private Canvas canvas;
    private TimeSpan lineTime = new TimeSpan(0,0,3);
    private bool playing;

    public event Action MessageFinished;
    public IObservable<Unit> MessageFinishedObservable => Observable.FromEvent(h => MessageFinished += h, h => MessageFinished -= h);
    

    // Start is called before the first frame update
    private void Awake()
    {
        text = gameObject.FirstOrDefault<TextMeshProUGUI>("text");
        nameText = gameObject.FirstOrDefault<TextMeshProUGUI>("name");
        canvas = GetComponentInChildren<Canvas>(true);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(playing) Hide();
        }
    }
    
    public void PlayText(string message, Action onClose)
    {
        playing = true;
        canvas.gameObject.SetActive(true);
        text.text = message;
        //nameText.text = 
        //
        // Observable.Timer(lineTime).Subscribe(l =>
        // {
        //     MessageFinished?.Invoke();
        //     Hide();
        // });

        _onClose = onClose;
    }

    public void Hide()
    {
        playing = false;
        _onClose?.Invoke();

    }

    private Action _onClose;

}