using Cysharp.Threading.Tasks;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class AudioExtensions
    {
        public static async UniTask PlayOneShotAsync(this UnityEngine.AudioSource audioSource, AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
            await UniTask.WaitWhile(() => audioSource.isPlaying);
        }
        
        
        
    }
}