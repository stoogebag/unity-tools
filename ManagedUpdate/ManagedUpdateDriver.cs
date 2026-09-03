using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stoogebag.ManagedUpdate
{
    /// <summary>
    /// The single native -> managed boundary for managed updates.
    /// One instance exists per scene. Each frame Unity calls its Update/FixedUpdate/
    /// LateUpdate once; it dispatches to every per-type manager. There is no per-object
    /// MonoBehaviour update - that is the whole point of the system.
    /// </summary>
    public class ManagedUpdateDriver : MonoBehaviour
    {
        public static ManagedUpdateDriver Instance { get; private set; }

        private readonly Dictionary<Type, ManagerBase> _managers = new Dictionary<Type, ManagerBase>();

        private void Awake() => Instance = this;

        private void OnDestroy()
        {
            _managers.Clear();
            Instance = null;
        }

        public void Register(MonoBehaviour obj)
        {
            if (obj == null) return;

            var type = obj.GetType();
            if (!_managers.TryGetValue(type, out var manager))
            {
                // Lazily manufacture the per-type manager (the old generator's "stub"),
                // closing the generic over the concrete component type at runtime.
                var managerType = typeof(ManagedUpdateManager<>).MakeGenericType(type);
                manager = (ManagerBase)Activator.CreateInstance(managerType);
                _managers[type] = manager;
            }

            manager.Add(obj);
        }

        public void Unregister(MonoBehaviour obj)
        {
            if (obj == null) return;

            var type = obj.GetType();
            if (_managers.TryGetValue(type, out var manager))
                manager.Remove(obj);
        }

        private void Update()
        {
            foreach (var manager in _managers.Values)
                manager.Update();
        }

        private void FixedUpdate()
        {
            foreach (var manager in _managers.Values)
                manager.FixedUpdate();
        }

        private void LateUpdate()
        {
            foreach (var manager in _managers.Values)
                manager.LateUpdate();
        }
    }
}
