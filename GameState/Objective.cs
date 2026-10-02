using UniRx;
using UnityEngine;

namespace stoogebag.GameState
{
    public abstract class Objective : MonoBehaviour, ISaveable
    {
        private readonly ReactiveProperty<bool> _active = new(false);
        private readonly ReactiveProperty<bool> _complete = new(false);
        private bool _destroyed;

        public IReadOnlyReactiveProperty<bool> IsActive => _active;
        public IReadOnlyReactiveProperty<bool> IsComplete => _complete;

        public Subject<Unit> OnStarted { get; } = new Subject<Unit>();
        public Subject<Unit> OnCompleted { get; } = new Subject<Unit>();

        public virtual float Progress => 0f;
        public virtual string ProgressText => string.Empty;

        protected void Begin()
        {
            if (_active.Value || _complete.Value || _destroyed)
            {
                return;
            }

            _active.Value = true;
            OnStarted.OnNext(Unit.Default);
        }

        protected void Complete()
        {
            if (_complete.Value)
            {
                return;
            }

            _complete.Value = true;
            _active.Value = false;
            OnCompleted.OnNext(Unit.Default);
        }

        public virtual object CaptureState()
        {
            return new ObjectiveState { Active = _active.Value, Complete = _complete.Value };
        }

        public virtual void RestoreState(object state)
        {
            if (state is not ObjectiveState saved)
            {
                return;
            }

            _complete.Value = saved.Complete;
            _active.Value = saved.Active && !saved.Complete;
        }

        protected virtual void OnDestroy()
        {
            _destroyed = true;
            _active.Dispose();
            _complete.Dispose();
            OnStarted.Dispose();
            OnCompleted.Dispose();
        }
    }

    public class ObjectiveState
    {
        public bool Active;
        public bool Complete;
        public int Count;
    }
}