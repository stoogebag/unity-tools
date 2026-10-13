using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using UnityEngine;
using USD.NET.Unity;
using Debug = UnityEngine.Debug;
using pxr;
using Unity.Formats.USD;
using UnityEditor;
using USD.NET;

public static class USDModelImporterPipeline
{
    static string ProjectRoot => Path.GetDirectoryName(Application.dataPath);

    static string ToAbsolute(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;
        return Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(ProjectRoot, path));
    }

    static string ResolveHython(string path)
    {
        if (!string.IsNullOrEmpty(path) && File.Exists(path)) return path;
        if (File.Exists("hython.exe")) return "hython.exe";
        var paths = Environment.GetEnvironmentVariable("PATH")?.Split(';') ?? new string[0];
        foreach (var p in paths)
        {
            var exe = Path.Combine(p.Trim(), "hython.exe");
            if (File.Exists(exe)) return exe;
        }
        return path;
    }

    static string ResolveProcessScript(string configured)
    {
        if (!string.IsNullOrEmpty(configured))
        {
            var abs = ToAbsolute(configured);
            if (File.Exists(abs)) return abs;
        }

        try
        {
            var found = Directory.GetFiles(Application.dataPath, "process_pipeline.py", SearchOption.AllDirectories);
            if (found.Length > 0) return found[0];
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"process_pipeline.py search failed: {ex.Message}");
        }

        return null;
    }

    static string PipelineStamp(GameObject go)
    {
        string sceneName = go.scene.name;
        string objectPath = go.GetPathInScene().Replace("/", "_");
        return sceneName + "_" + objectPath;
    }

    public static string DefaultOutputPath(this USDModelImporter importer)
    {
        return $"{importer.usdOutputFolder.TrimEnd('/')}/{PipelineStamp(importer.gameObject)}.usd";
    }

    public static async UniTask ExportAndProcess(this USDModelImporter importer, GameObject go, string assetName, CancellationToken ct = default)
    {
        string stamp = PipelineStamp(go);
        string pipelineFolderRel = $"{importer.pipelineRootFolder.TrimEnd('/')}/{stamp}";
        string pipelineFolder = pipelineFolderRel + "/";
        Directory.CreateDirectory(pipelineFolder);

        string inputPath = Path.Combine(pipelineFolder, "input.usd");
        string controlFilePath = Path.Combine(pipelineFolder, "control.json");
        string houdiniOutputPath = $"{importer.usdOutputFolder.TrimEnd('/')}/{stamp}.usd";
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(houdiniOutputPath)));

        go.SetActive(true);
        Export(go, inputPath);

        var controlData = new PipelineControlData
        {
            input_usd = inputPath,
            output_usd = houdiniOutputPath,
            status = "ready",
            timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff")
        };
        File.WriteAllText(controlFilePath, JsonUtility.ToJson(controlData, true));

        if (string.IsNullOrEmpty(importer.houdiniProject))
            throw new Exception($"Houdini project (hip) is not set on '{importer.name}'.");

        string absPipelineFolder = ToAbsolute(pipelineFolderRel);
        string absHipFile = ToAbsolute(importer.houdiniProject);

        string scriptPath = ResolveProcessScript(importer.processScriptPath);
        if (string.IsNullOrEmpty(scriptPath))
            throw new Exception("process_pipeline.py not found. Set 'processScriptPath' or ship it with the package.");

        var hython = ResolveHython(importer.hythonPath);
        var psi = new ProcessStartInfo(hython)
        {
            Arguments = $"\"{scriptPath}\" \"{absPipelineFolder}\" \"{absHipFile}\" \"{importer.houdiniNodePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            var proc = Process.Start(psi);
            string stdout = proc.StandardOutput.ReadToEnd();
            string stderr = proc.StandardError.ReadToEnd();
            proc.WaitForExit(30000);

            if (proc.ExitCode != 0)
            {
                Debug.LogError($"Houdini stdout:\n{stdout}");
                Debug.LogError($"Houdini stderr:\n{stderr}");
                throw new Exception($"Houdini pipeline failed with exit code {proc.ExitCode}");
            }

            Debug.Log($"Houdini stdout:\n{stdout}");
        }
        catch (System.ComponentModel.Win32Exception)
        {
            throw new Exception("Houdini (hython) not found. Is Houdini installed and on PATH?");
        }

        await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: ct);

        Import(importer, houdiniOutputPath);

        Debug.Log($"Pipeline complete: {houdiniOutputPath}");
    }

    public static void Import(this USDModelImporter importer, string path)
    {
        var transform = importer.transform;
        var gameObject = importer.gameObject;

        if (string.IsNullOrEmpty(path))
            path = DefaultOutputPath(importer);

        if (!File.Exists(path) && !File.Exists(ToAbsolute(path)))
        {
            Debug.LogError($"Processed USD not found: {path}");
            return;
        }

        ProcessedAsset previous = null;
        var parent = transform.parent;
        if (parent != null)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var pa = parent.GetChild(i).GetComponent<ProcessedAsset>();
                if (pa != null && pa.source == gameObject)
                {
                    previous = pa;
                    break;
                }
            }
        }
        if (previous != null)
            UnityEngine.Object.DestroyImmediate(previous.gameObject);

        var tagMap = new Dictionary<string, string>();
        {
            var readScene = ImportHelpers.InitForOpen(path, pxr.UsdStage.InitialLoadSet.LoadAll);
            if (readScene != null)
            {
                foreach (var prim in readScene.Stage.TraverseAll())
                {
                    var attr = prim.GetAttribute(new TfToken("primvars:HoudiniTags"));
                    if (attr == null || !attr.IsValid()) continue;
                    string tags = attr.Get(0);
                    if (!string.IsNullOrEmpty(tags))
                        tagMap[prim.GetPath().ToString()] = tags;
                }
                readScene.Close();
            }
        }

        Debug.Log($"[Import] TagMap has {tagMap.Count} entries");

        var tagToPrefab = new Dictionary<string, GameObject>();
        foreach (var ht in gameObject.GetComponentsInChildren<HoudiniTag>(true))
        {
            Debug.Log($"[Import] HoudiniTag on '{ht.gameObject.name}': tags=[{string.Join(",", ht.tags ?? new List<string>())}], prefab={ht.productionPrefab?.name ?? "NULL"}");
            if (ht.tags == null || ht.productionPrefab == null) continue;
            foreach (var t in ht.tags)
            {
                tagToPrefab[t] = ht.productionPrefab;
                Debug.Log($"[Import]   tagToPrefab['{t}'] = {ht.productionPrefab.name}");
            }
        }
        Debug.Log($"[Import] tagToPrefab has {tagToPrefab.Count} entries");

        var usdAsset = gameObject.GetComponentInChildren<UsdAsset>();
        var opts = new SceneImportOptions();
        if (usdAsset != null)
            usdAsset.StateToOptions(ref opts);

        var newScene = ImportHelpers.InitForOpen(path, pxr.UsdStage.InitialLoadSet.LoadAll);
        if (newScene == null)
        {
            Debug.LogError($"Failed to open USD: {path}");
            return;
        }

        var newRoot = ImportHelpers.ImportSceneAsGameObject(newScene, transform.parent?.gameObject, opts);
        if (newRoot == null)
        {
            Debug.LogError($"Failed to import: {path}");
            return;
        }

        newRoot.transform.SetPositionAndRotation(transform.position, transform.rotation);
        newRoot.transform.localScale = transform.lossyScale;
        newRoot.name = gameObject.name + "_PRODUCTION";

        Debug.Log("[Import] Walking imported tree for prefab matching...");

        var toReplace = new List<(Transform importTransform, GameObject prefabInstance)>();
        foreach (var t in newRoot.GetComponentsInChildren<Transform>(true))
        {
            var usdPS = t.GetComponent<UsdPrimSource>();
            if (usdPS == null) continue;
            if (!tagMap.TryGetValue(usdPS.m_usdPrimPath, out var primTags)) continue;
            if (string.IsNullOrEmpty(primTags)) continue;

            var parentTags = GetParentPrimTags(t, tagMap);
            var directTags = ComputeDirectTags(primTags, parentTags);

            Debug.Log($"[Import] GO '{t.name}' path={usdPS.m_usdPrimPath} primTags='{primTags}' parentTags='{parentTags ?? "NONE"}' directTags=[{string.Join(",", directTags)}]");

            if (directTags.Count == 0) continue;

            GameObject prefab = null;
            string matchedTag = null;
            foreach (var dtag in directTags)
            {
                if (tagToPrefab.TryGetValue(dtag, out prefab))
                {
                    matchedTag = dtag;
                    break;
                }
            }
            if (prefab == null)
            {
                Debug.Log($"[Import]   No prefab for directTags, skipping");
                continue;
            }

            Debug.Log($"[Import]   MATCHED tag '{matchedTag}' → prefab '{prefab.name}'");

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (instance == null)
            {
                Debug.LogError($"Failed to instantiate prefab: {prefab.name}");
                continue;
            }
            instance.name = prefab.name;

            var meshCount = PopulatePrefabMeshes(instance, t.gameObject);
            Debug.Log($"[Import]   Populated {meshCount} mesh(es) into prefab");
            toReplace.Add((t, instance));
        }

        Debug.Log($"[Import] Replacing {toReplace.Count} import GOs with prefab instances");
        foreach (var (importTransform, prefabInstance) in toReplace)
        {
            prefabInstance.transform.SetParent(importTransform.parent, worldPositionStays: false);
            prefabInstance.transform.SetSiblingIndex(importTransform.GetSiblingIndex());
            UnityEngine.Object.DestroyImmediate(importTransform.gameObject);
        }

        foreach (var ups in newRoot.GetComponentsInChildren<UsdPrimSource>(true))
            UnityEngine.Object.DestroyImmediate(ups);

        var resultTag = newRoot.AddComponent<ProcessedAsset>();
        resultTag.source = gameObject;

        if (importer.disableSourceOnImport)
            gameObject.SetActive(false);

        Debug.Log($"Imported: {newRoot.name}");
    }

    static string GetParentPrimTags(Transform t, Dictionary<string, string> tagMap)
    {
        var pt = t.parent;
        while (pt != null)
        {
            var ps = pt.GetComponent<UsdPrimSource>();
            if (ps != null && tagMap.TryGetValue(ps.m_usdPrimPath, out var tags))
                return tags;
            pt = pt.parent;
        }
        return null;
    }

    static List<string> ComputeDirectTags(string primTags, string parentTags)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(primTags)) return result;

        var own = primTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var parent = string.IsNullOrEmpty(parentTags)
            ? new HashSet<string>()
            : new HashSet<string>(parentTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

        foreach (var tag in own)
        {
            if (!parent.Contains(tag))
                result.Add(tag);
        }
        return result;
    }

    static int PopulatePrefabMeshes(GameObject prefabInstance, GameObject importGO)
    {
        int count = 0;
        foreach (var pt in prefabInstance.GetComponentsInChildren<Transform>(true))
        {
            var relPath = GetHierarchyPath(prefabInstance.transform, pt);
            if (relPath == null) continue;

            var match = importGO.transform.Find(relPath);
            if (match == null) continue;

            var srcMf = match.GetComponent<MeshFilter>();
            var dstMf = pt.GetComponent<MeshFilter>();
            if (srcMf != null && srcMf.sharedMesh != null)
            {
                if (dstMf == null) dstMf = pt.gameObject.AddComponent<MeshFilter>();
                dstMf.sharedMesh = srcMf.sharedMesh;
                var srcMr = match.GetComponent<MeshRenderer>();
                var dstMr = pt.GetComponent<MeshRenderer>();
                if (srcMr != null)
                {
                    if (dstMr == null) dstMr = pt.gameObject.AddComponent<MeshRenderer>();
                    dstMr.sharedMaterials = srcMr.sharedMaterials;
                }
                count++;
            }

            var srcSmr = match.GetComponent<SkinnedMeshRenderer>();
            if (srcSmr != null && srcSmr.sharedMesh != null)
            {
                var dstSmr = pt.GetComponent<SkinnedMeshRenderer>();
                if (dstSmr == null) dstSmr = pt.gameObject.AddComponent<SkinnedMeshRenderer>();
                dstSmr.sharedMesh = srcSmr.sharedMesh;
                dstSmr.sharedMaterials = srcSmr.sharedMaterials;
                dstSmr.bones = srcSmr.bones;
                dstSmr.rootBone = srcSmr.rootBone;
                count++;
            }
        }
        return count;
    }

    static string GetHierarchyPath(Transform root, Transform child)
    {
        if (child == root) return null;
        var path = child.name;
        var current = child.parent;
        while (current != null && current != root)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        return current == root ? path : null;
    }

    public static void ExportToDefaultFolder(this USDModelImporter importer)
    {
        ExportGameObject(importer.gameObject);
    }

    public static void ImportDefault(this USDModelImporter importer)
    {
        Import(importer, DefaultOutputPath(importer));
    }

    public static async void RunPipeline(this USDModelImporter importer)
    {
        await ExportAndProcess(importer, importer.gameObject, importer.gameObject.name);
    }

    [MenuItem("Tools/stoogebag/Houdini Pipeline/Export Selected USD")]
    static void ExportSelected()
    {
        ExportGameObject(Selection.activeGameObject);
    }

    static void ExportGameObject(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("No GameObject selected.");
            return;
        }

        string objName = CleanFileName(go.name);
        var importer = go.GetComponentInParent<USDModelImporter>();
        string dir = importer != null ? importer.usdExportFolder : USDModelImporter.DefaultUsdExportFolder;
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, objName + ".usda");

        Export(go, path);
    }

    static void Export(GameObject target, string path)
    {
        var scene = ExportHelpers.InitForSave(path);

        GameObject exportRoot = UnityEngine.Object.Instantiate(target);
        exportRoot.name = target.name;

        ExportGameObjectsWithAction(new[] { exportRoot }, scene, BasisTransformation.SlowAndSafe, true, gos =>
        {
            var root = gos[0];
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                var primPath = UnityTypeConverter.GetPath(t);
                var prim = scene.GetPrimAtPath(primPath);
                if (prim?.IsValid() != true) continue;

                var resolvedTags = new List<string>();
                var resolvedParams = new Dictionary<string, HoudiniParam>();
                string resolvedGuid = null;
                var current = t;
                while (current != null)
                {
                    var tag = current.GetComponent<HoudiniTag>();
                    if (tag != null)
                    {
                        if (tag.tags != null)
                            resolvedTags.AddRange(tag.tags);
                        if (tag.parameters != null)
                            foreach (var p in tag.parameters)
                                if (!string.IsNullOrEmpty(p.name) && !resolvedParams.ContainsKey(p.name))
                                    resolvedParams[p.name] = p;
                        if (resolvedGuid == null && !string.IsNullOrEmpty(tag.guid))
                            resolvedGuid = tag.guid;
                    }
                    current = current.parent;
                }

                if (resolvedTags.Count > 0)
                {
                    var joined = string.Join(",", resolvedTags);
                    var attrName = new TfToken("primvars:HoudiniTags");
                    var attr = prim.CreateAttribute(attrName, SdfValueTypeNames.String);
                    attr.Set(joined);
                }

                if (!string.IsNullOrEmpty(resolvedGuid))
                {
                    var guidAttr = prim.CreateAttribute(new TfToken("primvars:unity_guid"), SdfValueTypeNames.String);
                    guidAttr.Set(resolvedGuid);
                }

                foreach (var p in resolvedParams.Values)
                    WriteParam(prim, p);
            }
        });

        UnityEngine.Object.DestroyImmediate(exportRoot);
    }

    public static void ExportGameObjectsWithAction(GameObject[] objects, Scene scene,
        BasisTransformation basisTransform,
        bool exportMonoBehaviours = false, Action<GameObject[]> preSaveAction = null)
    {
        bool success = true;
        foreach (GameObject go in objects)
        {
            try
            {
                SceneExporter.Export(go, scene, basisTransform,
                    exportUnvarying: true, zeroRootTransform: false,
                    exportMonoBehaviours: exportMonoBehaviours);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                success = false;
            }
        }

        preSaveAction?.Invoke(objects);

        scene.Save();
        scene.Close();
    }

    static void WriteParam(pxr.UsdPrim prim, HoudiniParam p)
    {
        var attrName = new TfToken("primvars:unity_" + p.name);
        switch (p.type)
        {
            case HoudiniParamType.Int:
            {
                var attr = prim.CreateAttribute(attrName, SdfValueTypeNames.Int);
                attr.Set(int.TryParse(p.value, out var i) ? i : 0);
                break;
            }
            case HoudiniParamType.Float:
            {
                var attr = prim.CreateAttribute(attrName, SdfValueTypeNames.Float);
                attr.Set(float.TryParse(p.value, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var f) ? f : 0f);
                break;
            }
            case HoudiniParamType.Bool:
            {
                var attr = prim.CreateAttribute(attrName, SdfValueTypeNames.Bool);
                attr.Set(bool.TryParse(p.value, out var b) && b);
                break;
            }
            default:
            {
                var attr = prim.CreateAttribute(attrName, SdfValueTypeNames.String);
                attr.Set(p.value ?? "");
                break;
            }
        }
    }

    static string CleanFileName(string name)
    {
        return string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}

[Serializable]
class PipelineControlData
{
    public string input_usd;
    public string output_usd;
    public string status;
    public string timestamp;
}
