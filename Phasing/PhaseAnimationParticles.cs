using Cysharp.Threading.Tasks;
using UnityEngine;

namespace stoogebag.Phasing
{
    public class PhaseAnimationParticles : MonoBehaviour, IPhaseAnimation
    {
        public bool Reverse = true;
        private ParticleSystem ps;

        [SerializeField] private float duration = 4f;

        [SerializeField] private float startOffset = 0.1f;

        private void Awake()
        {
            ps = GetComponentInChildren<ParticleSystem>();
        }

        public async UniTask Animate()
        {
            ps = GetComponentInChildren<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var mainModule = ps.main;
            bool originalSeedMode = ps.useAutoRandomSeed;
            ps.useAutoRandomSeed = false;
            ps.randomSeed = (uint)gameObject.GetInstanceID();

            float currentSimulationTime = duration + startOffset;

            while (currentSimulationTime > startOffset)
            {
                currentSimulationTime -= Time.deltaTime;

                ps.Simulate(currentSimulationTime, true, true, false);

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.useAutoRandomSeed = originalSeedMode;
        }
    }
}
