#if UNITASK
#if TEXT_ANIMATOR

using Cysharp.Threading.Tasks;
using Febucci.TextAnimatorCore.Typing;
using Febucci.TextAnimatorForUnity.TextMeshPro;

namespace stoogebag.Extensions
{
    public static class TypewriterExtensions
    {
        public static UniTask ShowTextAndAwait(this TextAnimator_TMP typewriter, string text)
        {
            typewriter.SetText(text);
            return UniTask.WaitWhile(() => !typewriter.allLettersShown);
        }
        
        
    }
}
#endif
#endif