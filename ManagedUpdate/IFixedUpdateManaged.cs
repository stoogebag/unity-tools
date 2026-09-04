namespace Stoogebag.ManagedUpdate
{
    public interface IFixedUpdateManaged
    {
        void ManagedFixedUpdate();

        /// <summary>
        /// Called once per component type to create the per-type manager.
        /// </summary>
        ManagerBase CreateManager();
    }
}
