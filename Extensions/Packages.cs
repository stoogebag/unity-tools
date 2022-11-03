namespace stoogebag_MonuMental.stoogebag.Extensions
{
    public static class Packages
    {
        internal static void InstallUnityPackage(string packageName)
        {
#if UNITY_EDITOR
      
            UnityEditor.PackageManager.Client.Add($"com.unity.{packageName}");
      
#endif
        }
    }
}