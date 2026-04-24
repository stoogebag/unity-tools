#if UNITASK
#if TEXT_ANIMATOR

using Cysharp.Threading.Tasks;
using Febucci.UI.Core;

namespace stoogebag.Extensions
{
    public static class TypewriterExtensions
    {
        public static UniTask ShowTextAndAwait(this TypewriterCore typewriter, string text)
        {
            typewriter.ShowText(text);
            return UniTask.WaitWhile(() => typewriter.isShowingText);
        }
        
        
    }
}
#endif
#endif