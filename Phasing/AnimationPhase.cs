using System.Linq;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using UnityEngine;

namespace stoogebag.Phasing
{
    public class AnimationPhase : EntityPhase
    {
        public override async UniTask Enter()
        {
            await UniTask.WhenAll(gameObject.GetComponentsWithInterface<IPhaseAnimation>().Select(anim => anim.Animate()));

            Exit();
        }
    }
}
