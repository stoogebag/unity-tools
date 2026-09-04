using UnityEditor;

public static class ReloadLock
{
    [MenuItem("stooge/Tools/Reload/Lock Assemblies")]
    static void Lock() => EditorApplication.LockReloadAssemblies();

    [MenuItem("stooge/Tools/Reload/Unlock Assemblies")]
    static void Unlock() => EditorApplication.UnlockReloadAssemblies();
}