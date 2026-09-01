using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Stoogebag.ManagedUpdate.Editor
{
    public class ManagedUpdateBuildValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var invalidTypes = new List<Type>();

            var managedUpdateType = typeof(IManagedUpdate);
            var managedFixedUpdateType = typeof(IManagedFixedUpdate);
            var managedLateUpdateType = typeof(IManagedLateUpdate);
            var lifecycleType = typeof(ManagedUpdateLifecycle);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }

                foreach (var type in types)
                {
                    if (type == null) continue;
                    if (!type.IsSubclassOf(typeof(MonoBehaviour))) continue;
                    if (type.IsAbstract) continue;

                    bool implementsManagedUpdate =
                        managedUpdateType.IsAssignableFrom(type) ||
                        managedFixedUpdateType.IsAssignableFrom(type) ||
                        managedLateUpdateType.IsAssignableFrom(type);

                    if (!implementsManagedUpdate) continue;

                    if (!RequiresLifecycleComponent(type, lifecycleType))
                        invalidTypes.Add(type);
                }
            }

            if (invalidTypes.Count > 0)
            {
                var names = string.Join("\n", invalidTypes.Select(t => $"- {t.FullName}"));
                throw new BuildFailedException(
                    "The following MonoBehaviours implement a managed-update interface but are missing " +
                    $"[RequireComponent(typeof(ManagedUpdateLifecycle))]:\n\n{names}");
            }

            Debug.Log("[ManagedUpdate] Build validation passed.");
        }

        private static bool RequiresLifecycleComponent(Type type, Type lifecycleType)
        {
            var attributes = type.GetCustomAttributes(typeof(RequireComponent), true);

            foreach (RequireComponent require in attributes)
            {
                if (require.m_Type1 == lifecycleType ||
                    require.m_Type2 == lifecycleType ||
                    require.m_Type3 == lifecycleType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
