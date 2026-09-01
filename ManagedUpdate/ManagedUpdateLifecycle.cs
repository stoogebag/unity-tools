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
            if (UpdateDriver.Instance == null)
            {
                var go = new GameObject("UpdateDriver");
                go.AddComponent<UpdateDriver>();
            }

            FindManagedComponents();
            foreach (var component in _managedComponents)
                UpdateDriver.Instance?.Register(component);
        }

        private void OnDisable()
        {
            foreach (var component in _managedComponents)
                UpdateDriver.Instance?.Unregister(component);
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
                if (typeof(IManagedUpdate).IsAssignableFrom(type) ||
                    typeof(IManagedFixedUpdate).IsAssignableFrom(type) ||
                    typeof(IManagedLateUpdate).IsAssignableFrom(type))
                {
                    _managedComponents.Add(component);
                }
            }
        }
    }
}
