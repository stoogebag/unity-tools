using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace stoogebag.UITools.Windows
{
    public interface IWindowAnimation
    {
        public UniTask<bool> Activate(); //returns true if the activation completes successfully (ie without interruption)
        public UniTask<bool> Deactivate();
    }
}
