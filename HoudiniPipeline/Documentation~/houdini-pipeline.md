# Houdini Pipeline — detailed reference

## What it does

Takes a greyboxed GameObject tree, exports it to USD, runs a Houdini `.hip`
through `hython` to cook a produced version, then imports the result back into
Unity as a **new** sibling tree (`<Object>_PRODUCTION`) with production prefabs
swapped in where tags say so. The original greybox is left in place for
comparison.

The design is stateless: there is no per-instance state carried between runs.
Every run overwrites the previous artifacts by a deterministic name, and the
previous `_PRODUCTION` tree is deleted first.

## Requirements

- Unity 2022.3+.
- `com.unity.formats.usd` (Editor assemblies: `Unity.Formats.USD.Runtime`,
  `Unity.Formats.USD.Editor`, `Unity.Formats.USD.Common`, `USD.NET`,
  `USD.NET.Unity`).
- Houdini with `hython` on `PATH`, or an explicit path.
- `UniTask`, `stoogebag`.

## Assemblies

| Assembly | Platforms | Contents |
|---|---|---|
| `stoogebag.houdinipipeline` | all | `HoudiniTag`, `ProcessedAsset` |
| `stoogebag.houdinipipeline.editor` | Editor | `USDModelImporter`, `HoudiniPipelineEditor` |

The editor assembly references the runtime one, `stoogebag`, `UniTask`, and the
USD assemblies. It is gated on the `USDPIPELINE` define, which is contributed by
`versionDefines` when `com.unity.formats.usd` is installed. A project without
USD therefore skips the module entirely instead of failing to compile.

## Data flow

```
Unity greybox
   │  Export()   ── deep-copies the tree, writes primvars:HoudiniTags
   ▼
input.usd  +  control.json
   │  hython process_pipeline.py <folder> <hip> <nodePath>
   ▼
Houdini loads hip → sets unity_control parms → cooks usdexport node
   │
   ▼
output.usd   (written wherever control.json said)
   │  Import()
   ▼
<Object>_PRODUCTION tree, prefabs swapped in, ProcessedAsset on the root
```

Export and import both live in `Editor/USDModelImporter.cs`.

## Folder layout (defaults)

```
Assets/HoudiniPipeline/
  Pipeline/<Scene>_<ObjectPath>/
    input.usd
    control.json
  USD/<Scene>_<ObjectPath>.usd      (cook output)
  USD/Export/<Object>.usda          (manual Export button / menu)
```

`<ObjectPath>` is `GetPathInScene()` with `/` replaced by `_`, so it is stable
for a given hierarchy position. All three roots are configurable per component.

## control.json

Written next to `input.usd` before Houdini runs:

```json
{
  "input_usd": "Assets/HoudiniPipeline/Pipeline/Main_Root_Grid/input.usd",
  "output_usd": "Assets/HoudiniPipeline/USD/Main_Root_Grid.usd",
  "status": "ready",
  "timestamp": "20250121_150000_000"
}
```

`process_pipeline.py` reads it and copies `input_usd` and `output_usd` onto the
`unity_control` node's string parms (and passes the whole JSON through
`control_json`). Paths are relative to the project root.

## Houdini-side contract

At or under the configured **Houdini Node Path**:

- a node named `unity_control` with string parms `input_usd_path`,
  `output_usd_path`, `control_json`;
- a USD export node whose name contains `usdexport` (found by name first, then by
  a recursive name search).

`process_pipeline.py`:

1. loads the hip (`hou.hipFile.load`, ignoring load warnings);
2. resolves the parent node and `unity_control`;
3. sets `input_usd_path`, `output_usd_path`, `control_json`;
4. finds the export node and presses its `execute` parm.

## Tagging

`HoudiniTag` on a greybox object carries:

| Field | Meaning |
|---|---|
| `tags` | logical type names for this node, e.g. `GRASS` |
| `guid` | stable id (auto-generated in the editor) |
| `productionPrefab` | prefab to instantiate when this tag matches on import |

### How tags travel

Houdini is awkward to annotate per-prim, so the pipeline deliberately does not
require that. Instead, on export a prim receives **its own tags plus every
ancestor's tags**, joined with commas, as `primvars:HoudiniTags`:

```
parent HoudiniTag [GRID]  ──►  /root/Grid        primvars:HoudiniTags = "GRID"
  child  HoudiniTag [GRASS] ─► /root/Grid/Tile    primvars:HoudiniTags = "GRASS,GRID"
```

That way any subtree Houdini produces inherits a sensible type just by keeping
the attribute, without editing nodes per prim. The export uses
`SdfValueTypeNames.String` and is written in the `Export` pre-save pass.

### How tags are matched on import

The importer reads `primvars:HoudiniTags` for every prim into a map. It then
walks the imported GameObjects; for each one it looks up its prim's tag string
and the tag string of the nearest ancestor that also has one. Subtracting the
ancestor's tags leaves the node's **direct** tags:

```
prim  /root/Grid/Tile   tags "GRASS,GRID"
nearest tagged ancestor "GRID"
direct tags = { GRASS }
```

If any direct tag has a `productionPrefab` in the map built from the original
object's `HoudiniTag`s, that prefab replaces the imported mesh GameObject. A node
that only inherited its tags (no direct tag of its own) is left as-is.

## Import behaviour, step by step

1. Delete any previous sibling with a `ProcessedAsset` whose `source` is this
   object.
2. Read the tag map from `output.usd`.
3. Build tag → prefab from `GetComponentsInChildren<HoudiniTag>(true)` on the
   original.
4. Copy import options from the original's `UsdAsset` (`StateToOptions`) if one
   exists.
5. `ImportHelpers.ImportSceneAsGameObject` into a new tree.
6. Match the original's world position/rotation/scale; rename to
   `<Object>_PRODUCTION`.
7. For each transform with `UsdPrimSource`: compute direct tags; if a prefab
   matches, instantiate it and populate meshes, then queue a replacement.
8. Swap the prefab instances into the import tree (same parent, same sibling
   index) and destroy the originals.
9. Strip `UsdPrimSource` from the tree.
10. Add `ProcessedAsset(source = original)` to the root.
11. If `disableSourceOnImport`, deactivate the original.

### Prefab mesh population

For each transform in the prefab, the importer builds the path relative to the
prefab root (e.g. `Body/Legs`) and finds the same-named transform under the
imported prim. It copies `MeshFilter`/`MeshRenderer` or
`SkinnedMeshRenderer` (including bones/rootBone) onto the prefab's transform.
Matching is by name/path, so prefab child names must line up with the prim
hierarchy.

## Configuration

All fields live on the `USDModelImporter` component and therefore are per-object.

| Field | Default | Purpose |
|---|---|---|
| `houdiniProject` | (empty) | Path to the `.hip`. |
| `houdiniNodePath` | `/obj/geo1` | Parent node holding `unity_control`. |
| `hythonPath` | (empty) | Explicit `hython`; empty resolves from `PATH`. |
| `processScriptPath` | (empty) | Explicit driver; empty auto-finds the shipped script. |
| `pipelineRootFolder` | `Assets/HoudiniPipeline/Pipeline` | Where `input.usd`/`control.json` go. |
| `usdOutputFolder` | `Assets/HoudiniPipeline/USD` | Where the cook writes. |
| `usdExportFolder` | `Assets/HoudiniPipeline/USD/Export` | Where the manual export writes. |
| `disableSourceOnImport` | false | Deactivate the greybox after import. |

Resolving `process_pipeline.py`: the configured path is tried first, otherwise
`Application.dataPath` is searched recursively for `process_pipeline.py`. This
is why the driver ships in a `Python~` folder — it is found on disk, not through
the Asset Database.

## Editor UI

`HoudiniPipelineEditor` draws the fields with **…** browse buttons (file pickers
for hip/hython/script, folder pickers for the three roots) and the three
actions: **Export USD**, **Import Last Output**, **Export + Process**.

A global menu item `Tools > stoogebag > Houdini Pipeline > Export Selected USD`
uses the selected object's importer folder, falling back to the default export
folder.

## Failure modes and diagnostics

- **hython not found** — a `Win32Exception` is turned into a clear message.
- **Non-zero exit** — stdout/stderr are logged and an exception is thrown.
- **30 s cap** — the cook is waited on for 30 seconds; longer cooks are treated
  as failure. Streaming is sequential, so very large amounts of output can also
  stall the read.
- **Missing output** — `Import` logs `Processed USD not found` and returns.
- **No prefab for a direct tag** — logged and skipped; the plain mesh remains.
- **No `UsdAsset`** — import falls back to default `SceneImportOptions`.
- **Duplicate/`root` originals** — cleanup searches the original's parent only;
  scene-root objects can accumulate `_PRODUCTION` copies across runs.

## Portability notes

- Only three source files, one Python driver, and this documentation are needed;
  the hip/hda/USD content belongs to the consuming project.
- No Odin; the inspector is hand-written.
- The editor assembly's `versionDefines` gate means vendoring this into a project
  without USD is a no-op.
- Script GUIDs are stable across the move, so scene/prefab references survive
  extraction.
