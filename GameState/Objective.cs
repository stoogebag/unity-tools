using UniRx;
using UnityEngine;

namespace stoogebag.GameState
{
    public enum ObjectiveState
    {
        Unstarted,
        Running,
        Complete
    }

    /// <summary>
    /// A goal with a single state machine: Unstarted, Running, Complete.
    /// Completion is live — an objective can revert to Running unless it is
    /// <see cref="oneShot"/>.
    /// </summary>
    public abstract class Objective : MonoBehaviour, ISaveable
    {
        [SerializeField] private string _label;
        [SerializeField] private bool _oneShot;

        private readonly ReactiveProperty<ObjectiveState> _state = new(ObjectiveState.Unstarted);
        private bool _destroyed;

        public string Label => _label;
        public IReadOnlyReactiveProperty<ObjectiveState> State => _state;

        public bool IsUnstarted => _state.Value == ObjectiveState.Unstarted;
        public bool IsRunning => _state.Value == ObjectiveState.Running;
        public bool IsComplete => _state.Value == ObjectiveState.Complete;

        protected bool oneShot => _oneShot;

        public virtual float Progress => 0f;
        public virtual string ProgressText => string.Empty;

        public void Begin()
        {
            if (_destroyed || _state.Value != ObjectiveState.Unstarted)
                return;

            _state.Value = ObjectiveState.Running;
        }

        public void Satisfy()
        {
            if (_destroyed || _state.Value != ObjectiveState.Running)
                return;

            _state.Value = ObjectiveState.Complete;
        }

        public void Unsatisfy()
        {
            if (_destroyed || _oneShot || _state.Value != ObjectiveState.Complete)
                return;

            _state.Value = ObjectiveState.Running;
        }

        public virtual object CaptureState()
        {
            return new ObjectiveStateData { State = _state.Value };
        }

        public virtual void RestoreState(object state)
        {
            if (state is ObjectiveStateData data)
                _state.Value = data.State;
        }

        protected virtual void OnDestroy()
        {
            _destroyed = true;
            _state.Dispose();
        }
    }

    public class ObjectiveStateData
    {
        public ObjectiveState State;
    }
}
