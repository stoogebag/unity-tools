using System;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using UnityEngine;

namespace stoogebag.Phasing
{
    public class VfxPhase : EntityPhase
    {
        [SerializeField] private float timeInSeconds = 1f;

        public override async UniTask Enter()
        {
            gameObject.SetActive(true);

            var particles = gameObject.GetComponentInDescendants<ParticleSystem>(true);
            if (particles != null)
            {
                particles.Play();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(timeInSeconds));
            Exit();
        }
    }
}
