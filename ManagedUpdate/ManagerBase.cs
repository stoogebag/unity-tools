using UnityEngine;

namespace Stoogebag.ManagedUpdate
{
    public abstract class ManagerBase
    {
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void LateUpdate();
        public abstract void Add(MonoBehaviour obj);
        public abstract void Remove(MonoBehaviour obj);
    }
}
