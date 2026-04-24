#if UNITASK
#if TEXT_ANIMATOR

using Cysharp.Threading.Tasks;
using Febucci.TextAnimatorCore.Typing;
using Febucci.TextAnimatorForUnity;

namespace stoogebag.Extensions
{
    public static class TypewriterExtensions
    {
        public static async UniTask ShowTextAndAwait(this TypewriterCore typewriter, string text)
        {
            typewriter.ShowText(text);
            await UniTask.WaitForSeconds(1);
            await UniTask.WaitWhile(() => typewriter.IsShowingText);
        }
        
        
    }
}
#endif
#endif