using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITASK

namespace stoogebag.DIALOGUE
{
    public class Barker : MonoBehaviour
    {
        private UIPopup _uiPopup;
        private DialogueSpeaker _speaker;
    
        private void Awake()
        {
            _uiPopup = GetComponentInChildren<UIPopup>(true);
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
            _uiPopup.StartBark(line).Forget();

            if (line.Clip != null)
            {
                await _speaker.Play(line);
                //await UniTask.WaitUntil(() => !_audioSource.isPlaying);
            }
            else await UniTask.WaitForSeconds(2);

            await _uiPopup.Deactivate();
        }
    
    
    }
}
#endif