using System.Threading.Tasks;

namespace stoogebag_MonuMental.stoogebag.UITools.Windows
{
    public interface IWindowAnimation
    {
        public Task Activate();
        public Task Deactivate();
    }
}
