using Cysharp.Threading.Tasks;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class AudioExtensions
    {
        public static async UniTask PlayOneShotAsync(this UnityEngine.AudioSource audioSource, AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
            
            
            //await UniTask.WaitWhile(() => audioSource.isPlaying);
            
            //bc: this is weird, some clips length isn't correct if theyve been trimmed you see
            await UniTask.Delay((int) (clip.GetDuration() * 1000));
        }
        
        public static double GetDuration(this AudioClip clip)
        {
            
            return (double) clip.samples / clip.frequency;
        }
        
    }
}