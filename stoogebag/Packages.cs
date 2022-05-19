using System;
using UnityEditor;

namespace stoogebag
{
    public static class Packages
    {
        internal static void InstallUnityPackage(string packageName)
        {
            UnityEditor.PackageManager.Client.Add($"com.unity.{packageName}");
        }
    }
}