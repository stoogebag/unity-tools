using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace stoogebag.GameState
{
    /// <summary>
    /// An objective made of other objectives. Begins its children when it begins,
    /// and is Complete while they satisfy its rule. Reverts when they stop.
    /// </summary>
    public class ObjectiveGroup : Objective
    {
        public enum Rule { All, Any, Count }

        [SerializeField] private Rule rule = Rule.All;
        [SerializeField] private int count = 1;
        [SerializeField] private bool ordered;
        [SerializeField] private List<Objective> children = new();

        private readonly CompositeDisposable _subscriptions = new();

        private void OnEnable()
        {
            _subscriptions.Clear();
            Begin();

            foreach (var child in children)
            {
                if (child != null)
                    child.State.Subscribe(_ => Evaluate()).AddTo(_subscriptions);
            }

            Evaluate();
        }

        private void OnDisable()
        {
            _subscriptions.Clear();
        }

        private void Evaluate()
        {
            if (children.Count == 0)
                return;

            int satisfied = 0;
            bool chainBroken = false;

            foreach (var child in children)
            {
                if (child == null)
                    continue;

                bool complete = child.IsComplete;
                if (ordered && !complete)
                    chainBroken = true;

                if (ordered && chainBroken)
                    break;

                if (complete)
                    satisfied++;
            }

            bool met = rule switch
            {
                Rule.All => satisfied >= children.Count,
                Rule.Any => satisfied > 0,
                Rule.Count => satisfied >= count,
                _ => false
            };

            if (met)
                Satisfy();
            else
                Unsatisfy();
        }

        protected override void OnDestroy()
        {
            _subscriptions.Dispose();
            base.OnDestroy();
        }
    }
}
