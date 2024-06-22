#if CINEMACHINE
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    public string Name;
    
    //todo sprite, or something
    public async UniTask Play(DialogueLine dialogueLine)
    {
        await AudioSource.PlayOneShotAsync(dialogueLine.Clip);
    }

    public AudioSource AudioSource;
}
#endif