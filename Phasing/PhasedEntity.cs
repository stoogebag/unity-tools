using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using UnityEngine;

namespace stoogebag.Phasing
{
    public class PhasedEntity : MonoBehaviour
    {
        [SerializeField] private List<EntityPhase> phases;

        private EntityPhase currentPhase;

        [SerializeField] private EntityPhase overrideInitialPhase;

        [SerializeField] private bool BeginOnAwake = true;

        internal bool finished;

        private void Awake()
        {
            Init();
            if (BeginOnAwake) Begin();
        }

        private void Init()
        {
            var initialPhase = overrideInitialPhase != null ? overrideInitialPhase : phases[0];

            foreach (var entityPhase in phases)
            {
                entityPhase.gameObject.SetActive(initialPhase == entityPhase);
            }

            currentPhase = initialPhase;
        }

        public void Begin()
        {
            finished = false;
            var initialPhase = overrideInitialPhase != null ? overrideInitialPhase : phases[0];
            Init();
            initialPhase.gameObject.SetActive(true);
            initialPhase.Enter();
        }

        public void PhaseEnded(EntityPhase phase)
        {
            NextPhase();
        }

        public void NextPhase()
        {
            var index = phases.IndexOf(currentPhase);
            if (index == phases.Count - 1)
            {
                gameObject.SetActive(false);
                finished = true;
                return;
            }

            var oldPhase = phases[index];
            currentPhase = phases[index + 1];
            currentPhase.transform.position = oldPhase.transform.position;
            currentPhase.gameObject.SetActive(true);
            currentPhase.Enter();
        }
    }

    public abstract class EntityPhase : MonoBehaviour
    {
        public string PhaseName;

        public PhasedEntity Parent;

        private void Awake()
        {
            Parent = gameObject.GetComponentInAncestor<PhasedEntity>();
        }

        public abstract UniTask Enter();

        public void Exit()
        {
            gameObject.SetActive(false);
            Parent.PhaseEnded(this);
        }
    }
}
