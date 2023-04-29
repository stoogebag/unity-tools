using System.Threading.Tasks;

namespace stoogebag.UITools.Windows
{
    public interface IWindowAnimation
    {
        public Task<bool> Activate(); //returns true if the activation completes successfully (ie without interruption)
        public Task<bool> Deactivate();
    }
}
