using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;

namespace stoogebag.Extensions
{
    public static class TimelineExtensions
    {
        
        public static async UniTask PlayWithEndCallback(this PlayableDirector director, Action endAction)
        {
            director.Play();
            await UniTask.WaitWhile(() => director.time < director.duration);
            endAction?.Invoke();
        }

        public static async UniTask PlayAndAwait(this PlayableDirector director)
        {
            director.Play();
            await UniTask.WaitWhile(() => director.time < director.duration);
            //endAction?.Invoke();
        }

        
        
    }
}