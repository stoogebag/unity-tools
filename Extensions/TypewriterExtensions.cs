#if UNITASK
#if TEXT_ANIMATOR

using Cysharp.Threading.Tasks;
using Febucci.TextAnimatorCore.Typing;
using Febucci.TextAnimatorForUnity;
using Febucci.TextAnimatorForUnity.TextMeshPro;

namespace stoogebag.Extensions
{
    public static class TypewriterExtensions
    {
        public static async UniTask ShowTextAndAwait(this TypewriterComponent typewriter, string text)
        {
            typewriter.ShowText(text);
            await UniTask.WaitForSeconds(1);
            await UniTask.WaitWhile(() => typewriter.IsShowingText);
        }
        
        
    }
}
#endif
#endif