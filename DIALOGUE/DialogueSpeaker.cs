#if CINEMACHINE
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    public string Name;
    
    //todo sprite, or something
    public async UniTask Play(DialogueLine dialogueLine)
    {
        print("i am playing a dialogue");
    }
}
#endif