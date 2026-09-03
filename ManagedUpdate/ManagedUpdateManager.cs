using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Stoogebag.ManagedUpdate
{
    /// <summary>
    /// Per-type manager for a single managed-update component type T.
    /// Holds a homogeneous list of T and invokes its update methods directly (no
    /// per-item interface dispatch) via a reflection-built delegate. One instance
    /// is created lazily per concrete component type by ManagedUpdateDriver.
    /// </summary>
    public class ManagedUpdateManager<T> : ManagerBase where T : MonoBehaviour
    {
        private readonly List<T> _items = new List<T>();
        private readonly Action<T> _tickUpdate;
        private readonly Action<T> _tickFixed;
        private readonly Action<T> _tickLate;

        public ManagedUpdateManager()
        {
            _tickUpdate = MakeTick("ManagedUpdate", typeof(IUpdateManaged));
            _tickFixed = MakeTick("ManagedFixedUpdate", typeof(IFixedUpdateManaged));
            _tickLate = MakeTick("ManagedLateUpdate", typeof(ILateUpdateManaged));
        }

        private static Action<T> MakeTick(string methodName, Type interfaceType)
        {
            // Only build a tick if T actually implements this bucket's interface.
            // Without this, the fallback lambda below would blindly cast a component
            // that doesn't implement the interface and throw InvalidCastException.
            if (!interfaceType.IsAssignableFrom(typeof(T)))
                return null;

            var method = typeof(T).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
            {
                try
                {
                    // Direct (non-virtual) call to the concrete method.
                    return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), method);
                }
                catch
                {
                    // Fall through to interface dispatch.
                }
            }

            return t =>
            {
                if (interfaceType == typeof(IUpdateManaged))
                    ((IUpdateManaged)t).ManagedUpdate();
                else if (interfaceType == typeof(IFixedUpdateManaged))
                    ((IFixedUpdateManaged)t).ManagedFixedUpdate();
                else if (interfaceType == typeof(ILateUpdateManaged))
                    ((ILateUpdateManaged)t).ManagedLateUpdate();
            };
        }

        public override void Add(MonoBehaviour obj)
        {
            if (obj is T t) _items.Add(t);
        }

        public override void Remove(MonoBehaviour obj)
        {
            if (obj is T t) _items.Remove(t);
        }

        public override void Update()
        {
            Tick(_tickUpdate);
        }

        public override void FixedUpdate()
        {
            Tick(_tickFixed);
        }

        public override void LateUpdate()
        {
            Tick(_tickLate);
        }

        private void Tick(Action<T> tick)
        {
            if (tick == null) return;

            for (int i = _items.Count - 1; i >= 0; i--)
            {
                var item = _items[i];
                if (item == null) { _items.RemoveAt(i); continue; }
                tick(item);
            }
        }
    }
}
