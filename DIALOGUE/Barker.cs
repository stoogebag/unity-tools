#if UNITASK
#if ODIN_INSPECTOR
#if CINEMACHINE
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;


namespace stoogebag.DIALOGUE
{
    public class Barker : MonoBehaviour
    {
        private DialoguePanel _uiPopup;
        private DialogueSpeaker _speaker;
    
        private void Awake()
        {
            _uiPopup = gameObject.FirstOrDefault<DialoguePanel>();
            _speaker = GetComponent<DialogueSpeaker>();
        }

        [Button]
        void Test()
        {
            var line = new DialogueLine()
            {
                // Speaker = GetComponent<DialogueSpeaker>(),
            
                Text = "i'm barkin' here",
            };
            Bark(line).Forget();
        }
    
    
        public async UniTask Bark(DialogueLine line)
        {
            _uiPopup.Speaker = _speaker;
            var panelTask = _uiPopup.Bark(line, _speaker.Name);

            if (line.Clip != null)
            {
                await UniTask.WhenAll(panelTask, _speaker.Play(line));
                //await UniTask.WaitUntil(() => !_audioSource.isPlaying);
            }
            else await panelTask;

            await _uiPopup.Deactivate();
        }
    
    
    }
}
#endif
#endif
#endif