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
            await UniTask.WaitWhile(() => director.state == PlayState.Playing);
            endAction?.Invoke();
        }

        public static async UniTask PlayAndAwait(this PlayableDirector director)
        {
            director.Play();
            
            await UniTask.WaitWhile(() => director.state == PlayState.Playing);
            //endAction?.Invoke();
        }

        public static PlayableDirector GetDirector(this Playable playable)
        {
            return (playable.GetGraph().GetResolver() as PlayableDirector);
        }
        
    }
}