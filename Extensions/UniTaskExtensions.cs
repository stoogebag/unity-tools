#if UNITASK
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace stoogebag.Extensions
{
    public static class UniTaskExtensions
    {
        public static UniTask AwaitAllParallel(this IEnumerable<UniTask> tasks)
        {
            return UniTask.WhenAll(tasks);
        } 


        public static UniTask AwaitAllParallel(params UniTask[] tasks)
        {
            return UniTask.WhenAll(tasks);
        } 



        public static async UniTask AwaitAllSeries(this IEnumerable<UniTask> tasks)
        {
            foreach (var uniTask in tasks)
            {
                await uniTask;
            }
        } 
        
        public static async UniTask<bool> AwaitCondition(Func<bool> condition, int checkIntervalMs = 10, int timeoutMs = -1) //todo:timeout
        {
            while (!condition())
            {
                await UniTask.Delay(checkIntervalMs);
            }
            return true;
        }
        
    }
}
#endif