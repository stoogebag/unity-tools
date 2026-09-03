using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stoogebag.ManagedUpdate
{
    public class UpdateDriver : MonoBehaviour
    {
        public static UpdateDriver Instance { get; private set; }

        private readonly Dictionary<Type, ManagerBase> _managers = new();

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
                manager = ManagedUpdateTypeMap.CreateManager(type);
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
