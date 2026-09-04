#if UNITY_EDITOR


//from https://github.com/Walter-Hulsebos/com.lockyaw.compilation/tree/stable/Runtime
namespace Lockyaw.Compilation
{
    using System;

    using UnityEditor;
    using UnityEditor.Compilation;
    using UnityEngine;

    using Object = System.Object;

    /// <summary>
    /// Blocks Unity compilation by locking assembly reloads while active.
    /// Uses EditorPrefs for state, no EditorWindow needed.
    /// Exposes actions via Window/Lockyaw/Compilation and a Preferences page.
    /// </summary>
    [InitializeOnLoad]
    public static class CompilationBlocker
    {
        #region Pref Keys
        private const String PREFS_PREFIX           = "Lockyaw_CompilationBlocker_";
        private const String PREF_BLOCK_COMPILATION = PREFS_PREFIX + "BlockCompilation";
        #endregion

        #region State
        private static Boolean _blockCompilation;
        private static Boolean _lockHeld;
        private static Boolean _requestedCleanBuildOnce;
        private static Boolean _initialized;
        #endregion

        #region Static Init
        static CompilationBlocker()
        {
            EnsureInitialized();
            CompilationPipeline.compilationStarted += OnCompilationStarted;
        }
        #endregion

        #region Menu (Window/Lockyaw/Compilation)
        [MenuItem(itemName: "Window/Lockyaw/Compilation/Release + Refresh + Clean Compile")]
        private static void MenuReleaseRefreshCleanCompile()
        {
            EnsureInitialized();
            ReleaseRefreshCleanCompile();
        }

        [MenuItem(itemName: "Window/Lockyaw/Compilation/Disable Compilation")]
        private static void MenuDisableCompilation()
        {
            EnsureInitialized();
            SetBlockCompilation(block: true);
        }

        [MenuItem(itemName: "Window/Lockyaw/Compilation/Disable Compilation", isValidateFunction: true)]
        private static Boolean ValidateDisableCompilation()
        {
            EnsureInitialized();
            Menu.SetChecked(menuPath: "Window/Lockyaw/Compilation/Disable Compilation", isChecked: _blockCompilation);
            return true;
        }

        [MenuItem(itemName: "Window/Lockyaw/Compilation/Enable Compilation")]
        private static void MenuEnableCompilation()
        {
            EnsureInitialized();
            SetBlockCompilation(block: false);
        }

        [MenuItem(itemName: "Window/Lockyaw/Compilation/Enable Compilation", isValidateFunction: true)]
        private static Boolean ValidateEnableCompilation()
        {
            EnsureInitialized();
            Menu.SetChecked(menuPath: "Window/Lockyaw/Compilation/Enable Compilation", isChecked: !_blockCompilation);
            return true;
        }

        [MenuItem(itemName: "Window/Lockyaw/Compilation/Toggle Compilation")]
        private static void MenuToggleCompilation()
        {
            EnsureInitialized();
            SetBlockCompilation(block: !_blockCompilation);
        }

        [MenuItem(itemName: "Window/Lockyaw/Compilation/Open Preferences")]
        private static void MenuOpenPreferences()
        {
            SettingsService.OpenUserPreferences(settingsPath: "Preferences/Lockyaw/Compilation Blocker");
        }
        #endregion

        #region Preferences UI
        [SettingsProvider]
        private static SettingsProvider CreatePreferences()
        {
            SettingsProvider __provider = new (path: "Preferences/Lockyaw/Compilation Blocker", scopes: SettingsScope.User)
            {
                label = "Compilation Blocker",
                guiHandler = _ =>
                {
                    EnsureInitialized();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(label: "Compilation Blocker", style: EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox(message: "Blocks script compilation and assembly reload while active.", type: MessageType.Info);

                    Boolean __block = EditorGUILayout.Toggle(label: "Block Compilation", value: _blockCompilation);
                    if (__block != _blockCompilation) SetBlockCompilation(block: __block);

                    EditorGUILayout.Space();

                    // Force a single clean compile
                    if (GUILayout.Button(text: "Release + Refresh + Clean Compile"))
                    {
                        ReleaseRefreshCleanCompile();
                    }
                },
            };

            return __provider;
        }
        #endregion

        #region Core Logic
        private static void OnCompilationStarted(Object _)
        {
            #region Skip if not blocking compilation
            if (_blockCompilation == false) return;
            #endregion

            #region Lock and queue a single clean build
            if (_lockHeld == false)
            {
                EditorApplication.LockReloadAssemblies();
                _lockHeld = true;
            }

            if (_requestedCleanBuildOnce == false)
            {
                CompilationPipeline.RequestScriptCompilation(options: RequestScriptCompilationOptions.CleanBuildCache);
                _requestedCleanBuildOnce = true;
            }

            Debug.LogWarning(message: "[CompilationBlocker] Compilation blocked. Use Window/Lockyaw/Compilation/Release + Refresh + Clean Compile or Preferences.");
            #endregion
        }

        private static void EnsureInitialized()
        {
            if (_initialized) return;

            #region Load prefs (default: not blocking)
            _blockCompilation = EditorPrefs.GetBool(key: PREF_BLOCK_COMPILATION, defaultValue: false);
            _lockHeld = false;
            _requestedCleanBuildOnce = false;
            _initialized = true;
            #endregion

            #region Apply initial lock if needed
            if (_blockCompilation && _lockHeld == false)
            {
                EditorApplication.LockReloadAssemblies();
                _lockHeld = true;
            }
            #endregion
        }

        private static void SavePrefs()
        {
            EditorPrefs.SetBool(key: PREF_BLOCK_COMPILATION, value: _blockCompilation);
        }

        private static void SetBlockCompilation(Boolean block)
        {
            #region Update flag and save
            _blockCompilation = block;
            SavePrefs();
            #endregion

            #region Apply lock state immediately
            if (_blockCompilation)
            {
                if (_lockHeld == false)
                {
                    EditorApplication.LockReloadAssemblies();
                    _lockHeld = true;
                }
            }
            else
            {
                if (_lockHeld)
                {
                    EditorApplication.UnlockReloadAssemblies();
                    _lockHeld = false;
                }
            }

            // prepare a clean build request the first time compilation tries to start
            _requestedCleanBuildOnce = false;
            #endregion
        }

        private static void ReleaseRefreshCleanCompile()
        {
            #region Release block and unlock
            _blockCompilation = false;
            SavePrefs();

            if (_lockHeld)
            {
                EditorApplication.UnlockReloadAssemblies();
                _lockHeld = false;
            }
            _requestedCleanBuildOnce = false;
            #endregion

            #region Refresh assets first, then request a clean compile
            AssetDatabase.Refresh(options: ImportAssetOptions.ForceSynchronousImport);
            CompilationPipeline.RequestScriptCompilation(options: RequestScriptCompilationOptions.CleanBuildCache);

            Debug.Log(message: "[CompilationBlocker] Released, refreshed assets, and requested clean compile.");
            #endregion
        }
        #endregion
    }
}
#endif