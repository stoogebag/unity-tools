using Cysharp.Threading.Tasks;
using UnityEngine;

namespace stoogebag.Phasing
{
    public class TimedPhase : EntityPhase
    {
        [SerializeField] private float duration = 3f;

        public override async UniTask Enter()
        {
            await UniTask.WaitForSeconds(duration);
            if (gameObject != null) Exit();
        }
    }
}
