// using System.IO;
// using UnityEditor;
// using UnityEngine;
//
// namespace stoogebag.Editor
// {
//     [InitializeOnLoad]
//     public class AutoRefresher
//     {
//         private static FileSystemWatcher watcher;
//         private static bool doReload = false;
//         static AutoRefresher()
//         {
//             // FileSystemWatcher needs to be disposed after its work is done, so we disposing it before assembly reloads
//             AssemblyReloadEvents.beforeAssemblyReload += CleanUp;
//             EditorApplication.playModeStateChanged += OnPlayModeChanged;
//      
//             // settings for FileSystemWatcher, making shure it is capturing all changes in script files
//             watcher = new FileSystemWatcher(Path.GetFullPath(Application.dataPath));
//             watcher.Filter = "*.cs";
//             watcher.Changed += OnAssetsChanged;
//             watcher.Created += OnAssetsChanged;
//             watcher.Deleted += OnAssetsChanged;
//             watcher.Renamed += OnAssetsChanged;
//             watcher.IncludeSubdirectories = true;
//             watcher.EnableRaisingEvents = true;
//         }
//      
//         private static void OnAssetsChanged(object sender, FileSystemEventArgs e)
//         {
//             // if any script was changed then database should be reloaded before entering play mode
//             doReload = true;
//         }
//      
//         private static void OnPlayModeChanged(PlayModeStateChange state)
//         {
//             switch (state)
//             {
//                 case PlayModeStateChange.ExitingEditMode:
//                     OnPlayModeEntered();
//                     break;
//             }
//         }
//      
//         private static void CleanUp()
//         {
//             if (watcher != null)
//                 watcher.Dispose();
//         }
//      
//         private static void OnPlayModeEntered()
//         {
//             if (doReload)
//             {
//                 Debug.Log("asset database refresh");
//                 doReload = false;
//                 AssetDatabase.Refresh();
//             }
//         }
//     }
// }