namespace Stoogebag.ManagedUpdate
{
    public interface IUpdateManaged
    {
        void ManagedUpdate();

        /// <summary>
        /// Called once per component type to create the per-type manager.
        /// </summary>
        ManagerBase CreateManager();
    }
}
