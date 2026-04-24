using System;
using AxonGenesis;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeflowOneShot : MonoBehaviour
{
    [SerializeField] private bool playOnAwake = false;
    
    private void Awake()
    {
        if (playOnAwake) Play();
    }

    public async UniTask Play()
    {
        var tf = GetComponent<Timeflow>();
        tf.Play(true);
        
        await UniTask.WaitUntil(() => !tf.IsPlaying);
        
        tf.SetTime(0);
        
    }
}
