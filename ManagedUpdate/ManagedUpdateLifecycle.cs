using System.Collections.Generic;
using UnityEngine;

namespace Stoogebag.ManagedUpdate
{
    [DefaultExecutionOrder(-1000)]
    public class ManagedUpdateLifecycle : MonoBehaviour
    {
        private readonly List<MonoBehaviour> _managedComponents = new();

        private void OnEnable()
        {
            if (ManagedUpdateDriver.Instance == null)
            {
                var go = new GameObject("ManagedUpdateDriver");
                go.AddComponent<ManagedUpdateDriver>();
            }

            FindManagedComponents();
            foreach (var component in _managedComponents)
            {
                var type = component.GetType();
                if (!ManagedUpdateDriver.Instance.HasManager(type))
                {
                    var manager = CreateManagerFor(component);
                    if (manager != null)
                        ManagedUpdateDriver.Instance.RegisterManager(type, manager);
                }

                ManagedUpdateDriver.Instance.Register(component);
            }
        }

        private void OnDisable()
        {
            foreach (var component in _managedComponents)
                ManagedUpdateDriver.Instance?.Unregister(component);
        }

        private void OnDestroy()
        {
            OnDisable();
        }

        private void FindManagedComponents()
        {
            _managedComponents.Clear();
            var components = GetComponents<MonoBehaviour>();

            foreach (var component in components)
            {
                if (component == null) continue;

                var type = component.GetType();
                if (typeof(IUpdateManaged).IsAssignableFrom(type) ||
                    typeof(IFixedUpdateManaged).IsAssignableFrom(type) ||
                    typeof(ILateUpdateManaged).IsAssignableFrom(type))
                {
                    _managedComponents.Add(component);
                }
            }
        }

        private static ManagerBase CreateManagerFor(MonoBehaviour component)
        {
            if (component is IUpdateManaged u) return u.CreateManager();
            if (component is IFixedUpdateManaged f) return f.CreateManager();
            if (component is ILateUpdateManaged l) return l.CreateManager();
            return null;
        }
    }
}
