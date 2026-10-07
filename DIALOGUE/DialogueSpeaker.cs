

using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    public string Name;

    public Sprite Portrait;

    public async UniTask Play(DialogueLine dialogueLine)
    {
        await AudioSource.PlayOneShotAsync(dialogueLine.Clip);
    }

    [Button]
    void Bind()
    {
        AudioSource = GetComponentInChildren<AudioSource>();
    }
    
    public AudioSource AudioSource;
}
