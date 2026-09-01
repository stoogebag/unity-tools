using System;
using System.Collections.Generic;

namespace Stoogebag.ManagedUpdate
{
    public static class ManagedUpdateTypeMap
    {
        private static readonly Dictionary<Type, Func<ManagerBase>> Factories = new();

        public static void Register(Type type, Func<ManagerBase> factory)
        {
            Factories[type] = factory;
        }

        public static ManagerBase CreateManager(Type type)
        {
            if (!Factories.TryGetValue(type, out var factory))
                throw new ArgumentException(
                    $"No managed update manager registered for type {type}. " +
                    "Ensure the ManagedUpdate source generator has run and the assembly containing this type was compiled.",
                    nameof(type));

            return factory();
        }
    }
}
