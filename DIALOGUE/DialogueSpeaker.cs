#if CINEMACHINE
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
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

    [Button]
    void Bind()
    {
        AudioSource = GetComponentInChildren<AudioSource>();
    }
    
    public AudioSource AudioSource;
}
#endif