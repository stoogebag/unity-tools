using Cysharp.Threading.Tasks;

namespace stoogebag.Phasing
{
    public interface IPhaseAnimation
    {
        UniTask Animate();
    }
}
