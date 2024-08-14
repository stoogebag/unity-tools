// using System.IO;
// using UnityEditor;
// using UnityEngine;
//  
// namespace stoogebag.Editor {
//     [InitializeOnLoad]
//     public static class PlayRefresh {
//         private static bool _projectChanged;
//         private static readonly FileSystemWatcher _fileSystemWatcher;
//  
//         static PlayRefresh() {
//             EditorApplication.playModeStateChanged += Refresh;
//             EditorApplication.projectChanged += ProjectChanged;
//  
//             _fileSystemWatcher = new FileSystemWatcher();
//             _fileSystemWatcher.Path = Application.dataPath;
//             _fileSystemWatcher.IncludeSubdirectories = true;
//             _fileSystemWatcher.Filter = "*.cs;*.asmdef;*.inputactions";
//             _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
//  
//             _fileSystemWatcher.Changed += FileSystemChanged;
//  
//             _fileSystemWatcher.EnableRaisingEvents = true;
//         }
//  
//         private static void Refresh(PlayModeStateChange state) {
//             if (state == PlayModeStateChange.ExitingEditMode) {
//                 RefreshAssets();
//             }
//         }
//  
//         private static void ProjectChanged() {
//             _projectChanged = true;
//         }
//  
//         private static void FileSystemChanged(object sender, FileSystemEventArgs e) {
//             _projectChanged = true;
//         }
//  
//         [MenuItem("Tools/Auto Refresh")]
//         private static void AutoRefreshToggle() {
//             int status = EditorPrefs.GetInt("kAutoRefresh");
//             int newStatus = status == 1 ? 0 : 1;
//  
//             EditorPrefs.SetInt("kAutoRefresh", newStatus);
//  
//             if (newStatus == 1) EditorApplication.UnlockReloadAssemblies();
//             else EditorApplication.LockReloadAssemblies();
//  
//             CheckAutoRefresh();
//         }
//  
//         [MenuItem("Tools/Auto Refresh", true)]
//         private static bool AutoRefreshToggleValidation() {
//             int status = EditorPrefs.GetInt("kAutoRefresh");
//             Menu.SetChecked("Tools/Auto Refresh", status == 1);
//             return true;
//         }
//  
//         [MenuItem("Tools/Refresh %r")]
//         private static void Refresh() {
//             RefreshAssets();
//         }
//  
//         // This will executed after refresh
//         [InitializeOnLoadMethod]
//         private static void Initialize() {
//             if (EditorPrefs.HasKey("kAutoRefresh") == false) {
//                 EditorPrefs.SetInt("kAutoRefresh", 1);
//                 Menu.SetChecked("Tools/Auto Refresh", true);
//             }
//             int status = EditorPrefs.GetInt("kAutoRefresh");
//             if (status == 1) EditorApplication.UnlockReloadAssemblies();
//             else EditorApplication.LockReloadAssemblies();
//             CheckAutoRefresh();
//         }
//  
//         private static void RefreshAssets() {
//             Debug.Log($"Request refresh assets. (Project changed: {_projectChanged})");
//             if (!_projectChanged) return;
//             EditorApplication.UnlockReloadAssemblies();
//             AssetDatabase.Refresh();
//         }
//        
//         private static void CheckAutoRefresh() {
//             int status = EditorPrefs.GetInt("kAutoRefresh");
//             _projectChanged = status == 1; // If auto refresh is on, project changed is always true
//         }
//     }
// }
//  