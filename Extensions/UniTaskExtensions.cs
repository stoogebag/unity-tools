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
        public static async UniTask AwaitAllSeries(this IEnumerable<UniTask> tasks)
        {
            foreach (var uniTask in tasks)
            {
                await uniTask;
            }
        } 
        
    }
}