using Cysharp.Threading.Tasks;
using Febucci.UI.Core;

#if UNITASK
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