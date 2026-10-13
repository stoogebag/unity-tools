using Cysharp.Threading.Tasks;
using UnityEngine;

namespace stoogebag.Phasing
{
    public class PhaseDuration : MonoBehaviour
    {
        [SerializeField] private float duration = 10f;

        private void Start()
        {
            BeginPhase();
        }

        private async UniTask BeginPhase()
        {
            await UniTask.WaitForSeconds(duration);
            if (this != null) GetComponent<EntityPhase>().Exit();
        }
    }
}
