using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITASK

public class Barker : MonoBehaviour
{
    private UIPopup _uiPopup;
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _uiPopup = GetComponentInChildren<UIPopup>();
        _audioSource = GetComponentInChildren<AudioSource>();
    }

    [Button]
    void Test()
    {
        var line = new DialogueLine()
        {
            Speaker = GetComponent<DialogueSpeaker>(),
            
            Text = "i'm barkin' here",
        };
        Bark(line).Forget();
    }
    
    
    public async UniTask Bark(DialogueLine line)
    {
        _uiPopup.StartBark(line).Forget();

        if (line.Clip != null)
        {
            _audioSource.PlayOneShot(line.Clip);
            await UniTask.WaitUntil(() => !_audioSource.isPlaying);
        }
        else await UniTask.WaitForSeconds(2);

        await _uiPopup.Deactivate();
    }
    
    
}
#endif