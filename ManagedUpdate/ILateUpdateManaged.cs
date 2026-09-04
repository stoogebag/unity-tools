namespace Stoogebag.ManagedUpdate
{
    public interface ILateUpdateManaged
    {
        void ManagedLateUpdate();

        /// <summary>
        /// Called once per component type to create the per-type manager.
        /// </summary>
        ManagerBase CreateManager();
    }
}
