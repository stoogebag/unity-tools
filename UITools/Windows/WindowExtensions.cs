using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using stoogebag.UITools.Windows;

#if ODIN_INSPECTOR
#if UNITASK
public static class WindowExtensions
{

    public static async UniTask DeactivateAll(this IEnumerable<Window> windows)
    {
        await UniTask.WhenAll(windows.Select(w => w.Deactivate()));
    } 
    
    
    public static async UniTask ActivateAll(this IEnumerable<Window> windows)
    {
        await UniTask.WhenAll(windows.Select(w => w.Activate()));
    } 
    
}
#endif
#endif